using System.Text;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.Migrations.Base;
using BaSys.SuperAdmin.DAL.Abstractions;

namespace BaSys.Host.Services;

public class MigrationService : IMigrationService
{
    private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
    private readonly IBaSysConnectionFactory _baSysConnectionFactory;
    private readonly string? _dbName;
    
    public MigrationService(IDbInfoRecordsProvider dbInfoRecordsProvider,
        IBaSysConnectionFactory baSysConnectionFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbInfoRecordsProvider = dbInfoRecordsProvider;
        _baSysConnectionFactory = baSysConnectionFactory;
        _dbName = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "DbName")?.Value;
    }
    
    public List<MigrationBase>? GetMigrations()
    {
        var migrations = typeof(MigrationBase)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(MigrationBase)) && !t.IsAbstract)
            .Select(t => (MigrationBase)Activator.CreateInstance(t)!)
            .OrderBy(x => x.MigrationUtcIdentifier)
            .ToList();

        CheckMigrations(migrations);

        return migrations;
    }

    public async Task<List<MigrationBase>?> GetAppliedMigrations()
    {
        if (string.IsNullOrEmpty(_dbName))
            throw new ArgumentException("DbName not set!");
        
        var allMigrations = GetMigrations();
        
        var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(_dbName);
        using var connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord!.ConnectionString, dbInfoRecord.DbKind);
        var provider = new MigrationsProvider(connection);
        var appliedMigrationUids = (await provider.GetCollectionAsync(null))
            ?.Select(x => x.MigrationUid)
            .Distinct()
            .ToList();

        if (appliedMigrationUids == null)
            return new List<MigrationBase>();

        var appliedMigrations = allMigrations?.Where(x => appliedMigrationUids.Contains(x.Uid)).ToList();
        return appliedMigrations;
    }

    public async Task<bool> MigrationDown()
    {
        if (string.IsNullOrEmpty(_dbName))
            throw new ArgumentException("DbName not set!");
        
        var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(_dbName);
        using var connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord!.ConnectionString, dbInfoRecord.DbKind);
        var provider = new MigrationsProvider(connection);
        var lastMigration = (await provider.GetCollectionAsync(null))
            ?.OrderByDescending(x => x.ApplyDateTime)
            .FirstOrDefault();

        if (lastMigration == null)
            return false;
        
        // ToDo: execute migration down!!

        var state = await provider.DeleteAsync(lastMigration.Uid, null);
        return state == 1;
    }


    #region private methods
    private void CheckMigrations(List<MigrationBase> migrations)
    {
        // Same uids check
        var uidGroups = migrations.GroupBy(x => x.Uid);
        var sameUids = uidGroups.Where(x => x.Count() > 1).ToArray();
        if (sameUids.Any())
        {
            var sb = new StringBuilder();
            foreach (var sameUid in sameUids)
            {
                sb.Append($"{sameUid.Key},");
            }

            var str = sb.ToString().TrimEnd(',');
            throw new ArgumentException($"Duplicate migration uids: {str}");
        }
        
        // Same dates check
        var dateGroups = migrations.GroupBy(x => x.MigrationUtcIdentifier);
        var sameDates = dateGroups.Where(x => x.Count() > 1).ToArray();
        if (sameDates.Any())
        {
            var sb = new StringBuilder();
            foreach (var sameDate in sameDates)
            {
                sb.Append($"{sameDate.Key},");
            }

            var str = sb.ToString().TrimEnd(',');
            throw new ArgumentException($"Duplicate MigrationUtcIdentifier: {str}");
        }
        
        // Empty name check
        var emptyNameMigration = migrations.FirstOrDefault(x => string.IsNullOrEmpty(x.Name));
        if (emptyNameMigration != null)
            throw new ArgumentException($"Migration with uid: {emptyNameMigration.Uid} has no name!");
    }
    #endregion
}
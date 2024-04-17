using System.Text;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.Migrations.Base;
using BaSys.Host.DTO;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.SuperAdmin.DAL.Abstractions;

namespace BaSys.Host.Services;

public class MigrationService : IMigrationService
{
    private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
    private readonly IBaSysConnectionFactory _connectionFactory;
    private readonly string? _dbName;
    private readonly MigrationRunnerService _migrationRunnerService;
    private readonly LoggerService _loggerService;

    public MigrationService(IDbInfoRecordsProvider dbInfoRecordsProvider,
        IBaSysConnectionFactory connectionFactory,
        IHttpContextAccessor httpContextAccessor,
        MigrationRunnerService migrationRunnerService,
        LoggerService loggerService)
    {
        _dbInfoRecordsProvider = dbInfoRecordsProvider;
        _connectionFactory = connectionFactory;
        _dbName = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "DbName")?.Value;
        _migrationRunnerService = migrationRunnerService;
        _loggerService = loggerService;
    }

    public List<MigrationBase> GetMigrations()
    {
        var migrations = typeof(MigrationBase)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(MigrationBase)) && !t.IsAbstract)
            .Select(t => (MigrationBase) Activator.CreateInstance(t, _loggerService)!)
            .OrderByDescending(x => x.MigrationUtcIdentifier)
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
        using var connection =
            _connectionFactory.CreateConnection(dbInfoRecord!.ConnectionString, dbInfoRecord.DbKind);
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
    
    public async Task<List<MigrationDto>?> GetMigrationList()
    {
        var allMigrations = GetMigrations()
            ?.Select(x => new MigrationDto(x))
            .ToList();
        if (allMigrations == null)
            return null;

        var appliedMigrations = await GetAppliedMigrations();

        foreach (var appliedMigration in appliedMigrations ?? Enumerable.Empty<MigrationBase>())
        {
            var migraion = allMigrations.First(x => x.Uid == appliedMigration.Uid);
            migraion.IsApplied = true;
        }
        
        return allMigrations;
    }

    public async Task<bool> MigrationUp(Guid migrationUid)
    {
        if (string.IsNullOrEmpty(_dbName))
            throw new ArgumentException("DbName not set!");

        var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(_dbName);
        using var connection = _connectionFactory.CreateConnection(dbInfoRecord!.ConnectionString, dbInfoRecord.DbKind);
        var provider = new MigrationsProvider(connection);
        var dbMigration = await provider.GetMigrationByMigrationUidAsync(migrationUid, null);

        if (dbMigration != null)
            throw new ArgumentException($"Migration with uid: '{migrationUid}' already applied!");

        var notAppliedMigrations = await GetMigrationsToApply(migrationUid, provider);

        var state = _migrationRunnerService.MigrationUp(notAppliedMigrations);
        return state;
    }

    public bool StopMigration()
    {
        return _migrationRunnerService.StopMigration();
    }

    public bool GetMigrationStatus()
    {
        return _migrationRunnerService.IsMigrationRun();
    }

    public async Task<bool> MigrationDown()
    {
        if (string.IsNullOrEmpty(_dbName))
            throw new ArgumentException("DbName not set!");

        var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(_dbName);
        using var connection =
            _connectionFactory.CreateConnection(dbInfoRecord!.ConnectionString, dbInfoRecord.DbKind);
        var provider = new MigrationsProvider(connection);
        var lastMigration = (await provider.GetCollectionAsync(null))
            ?.OrderByDescending(x => x.ApplyDateTime)
            .FirstOrDefault();

        if (lastMigration == null)
            return false;

        var downMigration = GetMigrations()?.FirstOrDefault(x => x.Uid == lastMigration.MigrationUid);
        if (downMigration != null)
        {
            var state = _migrationRunnerService.MigrationDown(downMigration);
            return state;
        }

        return false;
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

    private async Task<List<MigrationBase>> GetMigrationsToApply(Guid migrationUid, MigrationsProvider provider)
    {
        var allMigrations = GetMigrations();
        var appliedMigrations = (await provider.GetCollectionAsync(null))
            .OrderBy(x => x.ApplyDateTime)
            .ToList();

        var notAppliedMigrations = new List<MigrationBase>();
        foreach (var migration in allMigrations ?? Enumerable.Empty<MigrationBase>())
        {
            if (appliedMigrations.Any(x => x.MigrationUid == migration.Uid))
                continue;
            
            notAppliedMigrations.Add(migration);
            if (migration.Uid == migrationUid)
                break;
        }
        
        return notAppliedMigrations;
    }

    #endregion
}
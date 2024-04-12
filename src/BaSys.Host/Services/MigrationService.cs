using System.Text;
using BaSys.DAL.Models;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.Migrations.Base;
using BaSys.Host.DTO;
using BaSys.SuperAdmin.DAL.Abstractions;

namespace BaSys.Host.Services;

public class MigrationService : IMigrationService
{
    private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
    private readonly IBaSysConnectionFactory _connectionFactory;
    private readonly string? _dbName;
    private readonly MigrationRunnerService _migrationRunnerService;

    public MigrationService(IDbInfoRecordsProvider dbInfoRecordsProvider,
        IBaSysConnectionFactory connectionFactory,
        IHttpContextAccessor httpContextAccessor,
        MigrationRunnerService migrationRunnerService)
    {
        _dbInfoRecordsProvider = dbInfoRecordsProvider;
        _connectionFactory = connectionFactory;
        _dbName = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "DbName")?.Value;
        _migrationRunnerService = migrationRunnerService;
    }

    public List<MigrationBase>? GetMigrations()
    {
        var migrations = typeof(MigrationBase)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(MigrationBase)) && !t.IsAbstract)
            .Select(t => (MigrationBase) Activator.CreateInstance(t)!)
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
        
        // foreach (var migration in notAppliedMigrations)
        // {
        //     // execute migration
        //     await migration.Up(connection);
        //
        //     await provider.InsertAsync(new Migration
        //     {
        //         Uid = Guid.NewGuid(),
        //         MigrationUid = migration.Uid,
        //         MigrationName = migration.Name!,
        //         ApplyDateTime = DateTime.UtcNow
        //     }, null);
        // }
        //
        // return new StartMigrationResultDto
        // {
        //     Result = true,
        //     RequestUid = Guid.NewGuid()
        // };
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
            
            // // execute down migration
            // await downMigration.Down(connection);
            //
            // var state = await provider.DeleteAsync(lastMigration.Uid, null);
            // if (state == 1)
            //     return new StartMigrationResultDto
            //     {
            //         Result = true,
            //         RequestUid = Guid.NewGuid()
            //     };
            // else
            //     return new StartMigrationResultDto();
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
        // var appliedMigrationUids = (await provider.GetCollectionAsync(null))
        //     ?.Select(x => x.MigrationUid)
        //     .Distinct()
        //     .ToList();
        //
        // var notAppliedMigrations = allMigrations!.Where(x => !appliedMigrationUids!.Contains(x.Uid))
        //     .OrderBy(x => x.MigrationUtcIdentifier)
        //     .ToList();
        //
        // return notAppliedMigrations;
        var l = new List<MigrationBase>();
        l.Add(allMigrations!.First());
        return l;
    }

    #endregion
}
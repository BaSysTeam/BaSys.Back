using System.Collections.Concurrent;
using System.Data;
using BaSys.DAL.Models;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.Migrations.Base;
using BaSys.SuperAdmin.DAL.Abstractions;

namespace BaSys.Host.Services;

internal class MigrationTask
{
    public Task? Task { get; set; }
    public CancellationTokenSource? CancellationTokenSource { get; set; }
}
public class MigrationRunnerService
{
    private readonly ConcurrentDictionary<string, MigrationTask> _runDict = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
    private readonly IBaSysConnectionFactory _connectionFactory;

    public MigrationRunnerService(IServiceProvider serviceProvider,
        IDbInfoRecordsProvider dbInfoRecordsProvider,
        IBaSysConnectionFactory connectionFactory)
    {
        _serviceProvider = serviceProvider;
        _dbInfoRecordsProvider = dbInfoRecordsProvider;
        _connectionFactory = connectionFactory;
    }

    public bool MigrationUp(IEnumerable<MigrationBase> migrations)
    {
        var dbName = GetDbName();
        if (string.IsNullOrEmpty(dbName))
            return false;
        
        if (IsMigrationRun(dbName))
            return false;
        
        var cts = new CancellationTokenSource();
        var task = Task.Factory.StartNew (async () =>
        {
            using var connection = GetConnection(dbName);
            var migrationsProvider = new MigrationsProvider(connection);
            
            foreach (var migration in migrations)
            {
                // execute migration
                await migration.Up(connection);
                await migrationsProvider.InsertAsync(new Migration
                {
                    Uid = Guid.NewGuid(),
                    MigrationUid = migration.Uid,
                    MigrationName = migration.Name!,
                    ApplyDateTime = DateTime.UtcNow
                }, null);
            }

            _runDict.TryRemove(dbName, out _);
        }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        
        _runDict.TryAdd(dbName, new MigrationTask
        {
            CancellationTokenSource = cts,
            Task = task
        });

        return true;
    }
    
    public bool MigrationDown(MigrationBase migration)
    {
        var dbName = GetDbName();
        if (string.IsNullOrEmpty(dbName))
            return false;
        
        if (IsMigrationRun(dbName))
            return false;

        var cts = new CancellationTokenSource();
        var task = Task.Factory.StartNew (async () =>
        {
            using var connection = GetConnection(dbName);
            
            await migration.Down(connection);
            
            var migrationsProvider = new MigrationsProvider(connection);
            await migrationsProvider.DeleteByMigrationUidAsync(migration.Uid);
            
            _runDict.TryRemove(dbName, out _);
        }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        
        _runDict.TryAdd(dbName, new MigrationTask
        {
            CancellationTokenSource = cts,
            Task = task
        });

        return true;
    }

    public bool StopMigration()
    {
        var dbName = GetDbName();
        if (string.IsNullOrEmpty(dbName))
            return false;
        
        if (!_runDict.TryGetValue(dbName.ToUpper(), out var migrationTask))
            return false;
        
        migrationTask.CancellationTokenSource?.Cancel();
        _runDict.TryRemove(dbName, out _);
        
        return true;
    }

    public bool IsMigrationRun(string dbName)
    {
        return _runDict.TryGetValue(dbName.ToUpper(), out _);
    }

    #region private methods
    private string? GetDbName()
    {
        var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
        return httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "DbName")?.Value;
    }

    private IDbConnection GetConnection(string dbName)
    {
        var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);
        return _connectionFactory.CreateConnection(dbInfoRecord!.ConnectionString, dbInfoRecord.DbKind);
    }
    #endregion
}
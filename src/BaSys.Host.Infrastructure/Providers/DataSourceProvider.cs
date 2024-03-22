using System.Collections.Concurrent;
using BaSys.Common.Enums;
using BaSys.Host.Infrastructure.Abstractions;
using BaSys.SuperAdmin.DAL;
using Microsoft.Extensions.DependencyInjection;

namespace BaSys.Host.Infrastructure.Providers;

public class DataSourceProvider : IDataSourceProvider
{
    private readonly List<ConnectionItem> _connectionItems = new();
    private ConcurrentDictionary<string, string> _userConnectionDict = new();
    private readonly IServiceProvider _serviceProvider;

    public DataSourceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Init();
    }

    public ConnectionItem? GetDefaultConnectionItem(DbKinds? dbKind = null)
    {
        if (dbKind == null)
            return _connectionItems.FirstOrDefault();

        return _connectionItems.FirstOrDefault(x => x.DbKind == dbKind);
    }

    public ConnectionItem? GetCurrentConnectionItemByUser(string? userId)
    {
        if (!string.IsNullOrEmpty(userId) && _userConnectionDict.TryGetValue(userId, out var connectionName))
            return _connectionItems.FirstOrDefault(x => x.Name == connectionName);

        return _connectionItems.FirstOrDefault();
    }

    public ConnectionItem? GetConnectionItemByDbName(string? dbName)
    {
        if (string.IsNullOrEmpty(dbName))
            return null;

        var item = _connectionItems.FirstOrDefault(x => x.Name.ToUpper() == dbName.ToUpper());
        return item;
    }

    public ConnectionItem? GetConnectionItemByDbInfoId(int dbInfoId)
    {
        if (_connectionItems.Any(x => x.Id == dbInfoId))
            return _connectionItems.First(x => x.Id == dbInfoId);

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SuperAdminDbContext>();
        var dbInfoRecord = context.DbInfoRecords.FirstOrDefault(x => x.Id == dbInfoId);
        if (dbInfoRecord == null)
            return null;
        
        var item = new ConnectionItem
        {
            Id = dbInfoRecord.Id,
            Name = dbInfoRecord.Name,
            ConnectionString = dbInfoRecord.ConnectionString,
            DbKind = dbInfoRecord.DbKind
        };
        _connectionItems.Add(item);

        return item;
    }
    
    public void SetConnection(string connectionName, string userId)
    {
        _userConnectionDict[userId] = connectionName;
    }

    public void RemoveConnection(string userId)
    {
        _userConnectionDict.TryRemove(userId, out _);
    }
    
    #region private methods
    private void Init()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SuperAdminDbContext>();
        
        try
        {
            var dbConnList = context.DbInfoRecords.ToList();
            _connectionItems.Clear();

            foreach (var conn in dbConnList)
            {
                _connectionItems.Add(new ConnectionItem
                {
                    Id = conn.Id,
                    Name = conn.Name,
                    ConnectionString = conn.ConnectionString,
                    DbKind = conn.DbKind
                });
            }
        }
        catch
        {
            // for success migration DbInfoRecords
        }
    }
    #endregion
}
using BaSys.Common.Enums;

namespace BaSys.Host.Infrastructure.Abstractions;

public interface IDataSourceProvider
{
    ConnectionItem? GetDefaultConnectionItem(DbKinds? dbKind = null);
    ConnectionItem? GetCurrentConnectionItemByUser(string? userId, string? dbName);
    ConnectionItem? GetConnectionItemByDbName(string? dbName);
    ConnectionItem? GetConnectionItemByDbInfoId(int dbInfoId);
    // void SetConnection(string connectionName, string userId);
    // void RemoveConnection(string userId);
}
using BaSys.Common.Enums;
using BaSys.Host.Infrastructure;

namespace BaSys.Host.Providers;

public interface IDataSourceProvider
{
    void Init();
    string? GetConnectionString(string? userId);
    List<ConnectionItem> GetConnectionItems();
    ConnectionItem? GetDefaultConnectionItem(DbKinds? dbKind = null);
    ConnectionItem? GetCurrentConnectionItemByUser(string? userId);
    ConnectionItem? GetConnectionItemByDbId(string? dbId);
    void SetConnection(string connId, string userId);
}
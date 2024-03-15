using BaSys.Common.Enums;

namespace BaSys.Host.Infrastructure.Interfaces;

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
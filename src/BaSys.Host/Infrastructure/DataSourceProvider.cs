using BaSys.Host.Providers;

namespace BaSys.Host.Infrastructure;

public class DataSourceProvider : IDataSourceProvider
{
    private readonly List<ConnectionItem> _connectionItems;
    private Dictionary<string, string> _userConnectionDict = new();
    
    public DataSourceProvider()
    {
        _connectionItems = new List<ConnectionItem>
        {
            new()
            {
                Id = "db1",
                ConnectionString =
                    "Data Source=OSPC\\SQLEXPRESS19;Initial Catalog=BaSysDb_1;Persist Security Info=True;User ID=sa;Password=QAZwsx!@#;TrustServerCertificate=True;",
                DbKind = DbKinds.MsSql
            },
            new()
            {
                Id = "db2",
                ConnectionString =
                    "Data Source=OSPC\\SQLEXPRESS19;Initial Catalog=BaSysDb_2;Persist Security Info=True;User ID=sa;Password=QAZwsx!@#;TrustServerCertificate=True;",
                DbKind = DbKinds.MsSql
            },
            new()
            {
                Id = "db3",
                ConnectionString = "Host=localhost;Port=5432;Database=BaSysDb_3;Username=postgres;Password=QAZwsx!@#",
                DbKind = DbKinds.PgSql
            }
        };
    }
    
    public string? GetConnectionString(string? userId)
    {
        var connectionItem = GetCurrentConnectionItemByUser(userId);
        return connectionItem?.ConnectionString;
    }

    public List<ConnectionItem> GetConnectionItems() => _connectionItems;

    public ConnectionItem? GetCurrentConnectionItemByUser(string? userId)
    {
        if (!string.IsNullOrEmpty(userId) && _userConnectionDict.TryGetValue(userId, out var connId))
            return _connectionItems.FirstOrDefault(x => x.Id == connId);

        return _connectionItems.FirstOrDefault();
    }

    public ConnectionItem? GetConnectionItemByDbId(string? dbId)
    {
        if (string.IsNullOrEmpty(dbId))
            return null;
        
        var item = _connectionItems.FirstOrDefault(x => x.Id.ToUpper() == dbId.ToUpper());
        return item;
    }

    public void SetConnection(string connId, string userId)
    {
        _userConnectionDict[userId] = connId;
    }
}
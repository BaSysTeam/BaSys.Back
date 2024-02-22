using BaSys.Common.Enums;

namespace BaSys.Host.Infrastructure;

public class ConnectionItem
{
    public string Id { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
    public DbKinds DbKind { get; set; }
}
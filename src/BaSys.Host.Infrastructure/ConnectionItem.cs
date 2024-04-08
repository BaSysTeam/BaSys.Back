using BaSys.Common.Enums;

namespace BaSys.Host.Infrastructure;

public class ConnectionItem
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
    public DbKinds DbKind { get; set; }
}
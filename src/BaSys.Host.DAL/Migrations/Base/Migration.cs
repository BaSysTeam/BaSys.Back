namespace BaSys.Host.DAL.Migrations.Base;

public abstract class Migration
{
    public abstract Guid Uid { get; }
    public abstract DateTime MigrationUtcIdentifier { get; }
    public abstract Task Up();
    public abstract Task Down();
}
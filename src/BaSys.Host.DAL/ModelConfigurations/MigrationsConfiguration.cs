using BaSys.DAL.Models;
using BaSys.FluentQueries.Models;

namespace BaSys.Host.DAL.ModelConfigurations;

public sealed class MigrationsConfiguration : DataModelConfiguration<Migration>
{
    public MigrationsConfiguration()
    {
        Table("sys_migrations");

        Column("uid").IsPrimaryKey();
        Column("MigrationUid");
        Column("ApplyDateTime");
    }
}
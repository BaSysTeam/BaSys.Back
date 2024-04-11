using BaSys.Host.DAL.Migrations.Base;

namespace BaSys.Host.Abstractions;

public interface IMigrationService
{
    List<MigrationBase>? GetMigrations();
    Task<List<MigrationBase>?> GetAppliedMigrations(string dbName);
}
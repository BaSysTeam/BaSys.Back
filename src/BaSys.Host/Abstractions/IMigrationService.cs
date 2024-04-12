using BaSys.Host.DAL.Migrations.Base;
using BaSys.Host.DTO;

namespace BaSys.Host.Abstractions;

public interface IMigrationService
{
    List<MigrationBase>? GetMigrations();
    Task<List<MigrationBase>?> GetAppliedMigrations();
    Task<bool> MigrationDown();
    Task<bool> MigrationUp(Guid migrationUid);
    bool StopMigration();
}
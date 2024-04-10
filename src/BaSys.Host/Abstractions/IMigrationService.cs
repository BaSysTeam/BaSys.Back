using BaSys.Host.DAL.Migrations.Base;

namespace BaSys.Host.Abstractions;

public interface IMigrationService
{
    List<Migration>? GetMigrations();
}
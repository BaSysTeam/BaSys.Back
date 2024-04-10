using BaSys.Host.DAL.Migrations.Base;

namespace BaSys.Host.DTO;

public class MigrationDto
{
    public MigrationDto()
    {
    }

    public MigrationDto(Migration migration)
    {
        Uid = migration.Uid;
        MigrationUtcIdentifier = migration.MigrationUtcIdentifier;
        Name = migration.Name ?? string.Empty;
        Description = migration.Description;
    }
    
    public Guid Uid { get; set; }
    public DateTime MigrationUtcIdentifier { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}
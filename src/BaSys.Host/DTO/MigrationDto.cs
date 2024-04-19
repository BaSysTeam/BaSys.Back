using BaSys.Host.DAL.Migrations.Base;

namespace BaSys.Host.DTO;

public class MigrationDto
{
    public MigrationDto()
    {
    }

    public MigrationDto(MigrationBase migrationBase)
    {
        Uid = migrationBase.Uid;
        MigrationUtcIdentifier = migrationBase.MigrationUtcIdentifier;
        Name = migrationBase.Name ?? string.Empty;
        Description = migrationBase.Description;
    }
    
    public Guid Uid { get; set; }
    public DateTime MigrationUtcIdentifier { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsApplied { get; set; }
    public bool IsPossibleRemove { get; set; }
}
using BaSys.Host.DAL.Migrations.Base;

namespace BaSys.Host.DAL.Migrations;

public class Migration3 : MigrationBase
{
    public override Guid Uid => new("28f69c13-baa2-4b6c-98fa-4752275deca5");
    public override DateTime MigrationUtcIdentifier => new (2024, 4, 10, 12, 30, 0);
    
    public override string Name => "Migration3";
    
    public override async Task Up()
    {
        await Task.Delay(3000);
    }

    public override async Task Down()
    {
        await Task.Delay(3000);
    }
}
using BaSys.Host.DAL.Migrations.Base;

namespace BaSys.Host.DAL.Migrations;

public class Migration2 : MigrationBase
{
    public override Guid Uid => new("22269c13-baa2-5b1c-98fa-4752275deca5");
    public override DateTime MigrationUtcIdentifier => new (2024, 4, 5, 12, 30, 0);
    
    public override string Name => "Migration2";

    public override async Task Up()
    {
        await Task.Delay(3000);
    }

    public override async Task Down()
    {
        await Task.Delay(3000);
    }
}
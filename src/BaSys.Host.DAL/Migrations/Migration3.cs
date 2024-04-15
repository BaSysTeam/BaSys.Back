using System.Data;
using BaSys.Host.DAL.Migrations.Base;
using BaSys.Logging.Abstractions.Abstractions;

namespace BaSys.Host.DAL.Migrations;

public class Migration3 : MigrationBase
{
    public Migration3(LoggerService loggerService) : base(loggerService)
    {
    }
    
    public override Guid Uid => new("33369c13-baa2-4b6c-98fa-4752275deca5");
    public override DateTime MigrationUtcIdentifier => new (2024, 4, 10, 12, 30, 0);
    
    public override string Name => "Migration3";
    
    public override async Task Up(IDbConnection connection, CancellationToken ct)
    {
        await Task.Delay(120000, ct);
    }

    public override async Task Down(IDbConnection connection, CancellationToken ct)
    {
        await Task.Delay(1000, ct);
    }
}
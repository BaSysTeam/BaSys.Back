using System.Data;
using BaSys.Host.DAL.Migrations.Base;
using BaSys.Logging.Abstractions.Abstractions;

namespace BaSys.Host.DAL.Migrations;

public class Migration2 : MigrationBase
{
    public Migration2(ILoggerService loggerService) : base(loggerService)
    {
    }
    
    public override Guid Uid => new("22269c13-baa2-5b1c-98fa-4752275deca5");
    public override DateTime MigrationUtcIdentifier => new (2024, 4, 5, 12, 30, 0);
    
    public override string Name => "Migration2";
    public override string Description => "Migration2 description";

    public override async Task Up(IDbConnection connection, CancellationToken ct)
    {
        await Task.Delay(1000, ct);
    }

    public override async Task Down(IDbConnection connection, CancellationToken ct)
    {
        await Task.Delay(1000, ct);
    }
}
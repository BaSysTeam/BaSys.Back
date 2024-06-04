using System.Data;
using BaSys.Logging.Abstractions.Abstractions;

namespace BaSys.Host.DAL.Migrations.Base;

public abstract class MigrationBase
{
    protected readonly ILoggerService _loggerService;
    public MigrationBase(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }
    
    public abstract Guid Uid { get; }
    public abstract DateTime MigrationUtcIdentifier { get; }
    public abstract string Name { get; }
    public virtual string? Description { get; }
    public abstract Task Up(IDbConnection connection, CancellationToken ct);
    public abstract Task Down(IDbConnection connection, CancellationToken ct);
}
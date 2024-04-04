using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.Abstractions.Enums;
using Serilog;

namespace BaSys.Logging.LogServices;

public class MongoLoggerService : LoggerService
{
    public MongoLoggerService(LoggerConfig loggerConfig) : base(loggerConfig)
    {
        _logger = new LoggerConfiguration()
            .WriteTo
            .MongoDB(loggerConfig.ConnectionString, loggerConfig.TableName)
            .CreateLogger();
    }

    protected override void WriteInner(string message, EventTypeLevels level, EventType eventType)
    {
        _logger.Information("{message} {Level} {EventTypeName} {EventTypeUid} {Module}",
            message,
            (int)level,
            eventType.EventName,
            eventType.Uid,
            eventType.Module);
    }

    public override void Dispose() => _logger.Dispose();
}
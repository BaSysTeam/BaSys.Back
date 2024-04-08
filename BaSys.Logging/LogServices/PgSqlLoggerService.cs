using BaSys.Common.Enums;
using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using NpgsqlTypes;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;

namespace BaSys.Logging.LogServices;

public class PgSqlLoggerService : LoggerService
{
    public PgSqlLoggerService(LoggerConfig loggerConfig) : base(loggerConfig)
    {
        var columnOptions = new Dictionary<string, ColumnWriterBase>
        {
            {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            // {"timestamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
            // {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            // {"level", new SinglePropertyColumnWriter("Level", PropertyWriteMethod.Raw, NpgsqlDbType.Integer) },
            // {"event_type_uid", new SinglePropertyColumnWriter("EventTypeUid", PropertyWriteMethod.Raw, NpgsqlDbType.Uuid) },
            // {"event_type_name", new SinglePropertyColumnWriter("EventTypeName", PropertyWriteMethod.ToString, NpgsqlDbType.Text) },
            // {"module", new SinglePropertyColumnWriter("Module", PropertyWriteMethod.ToString, NpgsqlDbType.Text) }
        };
        
        _logger = new LoggerConfiguration()
            .WriteTo
            .PostgreSQL(connectionString: loggerConfig.ConnectionString,
                tableName: loggerConfig.TableName,
                columnOptions: columnOptions,
                needAutoCreateTable: true)
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
}
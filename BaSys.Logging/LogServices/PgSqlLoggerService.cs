using BaSys.Common.Enums;
using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using Serilog.Sinks.PostgreSQL.EventArgs;

namespace BaSys.Logging.LogServices;

public class PgSqlLoggerService : LoggerService
{
    public PgSqlLoggerService(LoggerConfig loggerConfig) : base(loggerConfig)
    {
        if (string.IsNullOrEmpty(loggerConfig.ConnectionString) || string.IsNullOrEmpty(loggerConfig.TableName))
            throw new ArgumentException();

        // var columnOptions = new Dictionary<string, ColumnWriterBase>
        // {
        //     {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
        //     // {"timestamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
        //     // {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
        //     // {"level", new SinglePropertyColumnWriter("Level", PropertyWriteMethod.Raw, NpgsqlDbType.Integer) },
        //     // {"event_type_uid", new SinglePropertyColumnWriter("EventTypeUid", PropertyWriteMethod.Raw, NpgsqlDbType.Uuid) },
        //     // {"event_type_name", new SinglePropertyColumnWriter("EventTypeName", PropertyWriteMethod.ToString, NpgsqlDbType.Text) },
        //     // {"module", new SinglePropertyColumnWriter("Module", PropertyWriteMethod.ToString, NpgsqlDbType.Text) }
        // };

        CheckDbExists(loggerConfig.ConnectionString);

        var columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
            {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text)},
            {"level", new LevelColumnWriter(true, NpgsqlDbType.Varchar)},
            {"raise_date", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
            {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
            {"properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb)}
        };

        try
        {
            _logger = new LoggerConfiguration()
                .WriteTo
                .PostgreSQL(connectionString: loggerConfig.ConnectionString,
                    tableName: loggerConfig.TableName,
                    columnOptions: columnWriters,
                    needAutoCreateTable: true)
                .CreateLogger();
        }
        catch
        {
        }
    }

    private void CheckDbExists(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        var dbName = builder.Database;
        builder.Remove("Database");
        builder.CommandTimeout = 1;
        builder.Timeout = 1;

        try
        {
            using var db = new NpgsqlConnection(builder.ConnectionString);
            var dbId = db.QueryFirstOrDefault($"SELECT oid FROM pg_catalog.pg_database WHERE lower(datname) = lower('{dbName}');");

            if (dbId != null)
                return;
            
            // create db
            var sql = @$"CREATE DATABASE serilog
                    WITH
                        OWNER = {builder.Username}
                        ENCODING = 'UTF8'
                        LOCALE_PROVIDER = 'libc'
                        CONNECTION LIMIT = -1
                        IS_TEMPLATE = False;";
            db.Execute(sql);
        }
        catch
        {
        }
    }

    protected override void WriteInner(string message, EventTypeLevels level, EventType eventType)
    {
        _logger?.Information("{message} {Level} {EventTypeName} {EventTypeUid} {Module}",
            message,
            (int) level,
            eventType.EventName,
            eventType.Uid,
            eventType.Module);
    }
}
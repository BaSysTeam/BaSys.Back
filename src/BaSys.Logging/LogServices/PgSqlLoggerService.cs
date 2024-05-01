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
    public PgSqlLoggerService(LoggerConfig loggerConfig, string? userUid, string? userName, string? ipAddress) 
        : base(loggerConfig, userUid, userName, ipAddress)
    {
        if (string.IsNullOrEmpty(loggerConfig.ConnectionString) || string.IsNullOrEmpty(loggerConfig.TableName))
            return;

        CheckDbExists(loggerConfig.ConnectionString);

        var columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
            {"exception_message", new SinglePropertyColumnWriter("ExceptionMessage", PropertyWriteMethod.Raw)},
            {"raise_date", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
            {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
            {"level", new SinglePropertyColumnWriter("Level", PropertyWriteMethod.Raw, NpgsqlDbType.Integer)},
            {"event_type_uid", new SinglePropertyColumnWriter("EventTypeUid", PropertyWriteMethod.Raw, NpgsqlDbType.Uuid)},
            {"event_type_name", new SinglePropertyColumnWriter("EventTypeName")},
            {"module", new SinglePropertyColumnWriter("Module")},
            {"user_uid", new SinglePropertyColumnWriter("UserUid")},
            {"user_name", new SinglePropertyColumnWriter("UserName")},
            {"ip_address", new SinglePropertyColumnWriter("IpAddress")},
            {"properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb)},
            {"metadata_uid", new SinglePropertyColumnWriter("MetadataUid", PropertyWriteMethod.Raw, NpgsqlDbType.Uuid)},
            {"data_uid", new SinglePropertyColumnWriter("DataUid", PropertyWriteMethod.Raw, NpgsqlDbType.Varchar)},
            {"data_presentation", new SinglePropertyColumnWriter("DataPresentation", PropertyWriteMethod.Raw, NpgsqlDbType.Varchar)}
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

    protected override void WriteInner(string? message,
        EventTypeLevels level,
        EventType eventType,
        string? exception = null,
        Guid? metadataUid = null,
        string? dataUid = null,
        string? dataPresentation = null)
    {
        _logger?.Information("{message} {ExceptionMessage} {Level} {EventTypeName} {EventTypeUid} {Module} {UserUid} {UserName} {IpAddress} {MetadataUid} {DataUid} {DataPresentation}",
            message,
            exception,
            (int) level,
            eventType.EventName,
            eventType.Uid,
            eventType.Module,
            _userUid,
            _userName,
            _ipAddress,
            metadataUid,
            dataUid,
            dataPresentation);
    }
}
using System.Data;
using BaSys.Common.Enums;
using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace BaSys.Logging.LogServices;

public class MsSqlLoggerService : LoggerService
{
    public MsSqlLoggerService(LoggerConfig loggerConfig, string? userUid, string? userName, string? ipAddress) 
        : base(loggerConfig, userUid, userName, ipAddress)
    {
        var sinkOpts = new MSSqlServerSinkOptions();
        sinkOpts.AutoCreateSqlDatabase = true;
        sinkOpts.AutoCreateSqlTable = true;
        sinkOpts.TableName = loggerConfig.TableName;
        
        var columnOptions = new ColumnOptions();
        columnOptions.Store.Remove(StandardColumn.Level);
        columnOptions.Store.Remove(StandardColumn.MessageTemplate);
        columnOptions.Store.Remove(StandardColumn.Properties);
        
        columnOptions.AdditionalColumns = new List<SqlColumn>();
        columnOptions.AdditionalColumns.Add(new SqlColumn()
        {
            ColumnName = "Level",
            DataType = SqlDbType.Int
        });
        columnOptions.AdditionalColumns.Add(new SqlColumn()
        {
            ColumnName = "EventTypeUid",
            DataType = SqlDbType.UniqueIdentifier
        });
        columnOptions.AdditionalColumns.Add(new SqlColumn()
        {
            ColumnName = "EventTypeName",
            DataType = SqlDbType.NVarChar,
            DataLength = 100
        });
        columnOptions.AdditionalColumns.Add(new SqlColumn()
        {
            ColumnName = "Module",
            DataType = SqlDbType.NVarChar,
            DataLength = 100
        });
        columnOptions.AdditionalColumns.Add(new SqlColumn()
        {
            ColumnName = "UserUid",
            DataType = SqlDbType.NVarChar,
            DataLength = 100
        });
        columnOptions.AdditionalColumns.Add(new SqlColumn()
        {
            ColumnName = "UserName",
            DataType = SqlDbType.NVarChar,
            DataLength = 100
        });
        columnOptions.AdditionalColumns.Add(new SqlColumn()
        {
            ColumnName = "IpAddress",
            DataType = SqlDbType.NVarChar,
            DataLength = 100
        });

        try
        {
            _logger = new LoggerConfiguration()
                .WriteTo
                .MSSqlServer(connectionString: loggerConfig.ConnectionString, sinkOptions: sinkOpts, columnOptions: columnOptions)
                .CreateLogger();
        }
        catch
        {
        }
    }
    
    protected override void WriteInner(string message, EventTypeLevels level, EventType eventType)
    {
        _logger?.Information("{message} {Level} {EventTypeName} {EventTypeUid} {Module} {UserUid} {UserName} {IpAddress}",
            message,
            (int)level,
            eventType.EventName,
            eventType.Uid,
            eventType.Module,
            _userUid,
            _userName,
            _ipAddress);
    }
}
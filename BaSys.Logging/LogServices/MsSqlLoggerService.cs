using System.Data;
using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.Abstractions.Enums;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.MSSqlServer;

namespace BaSys.Logging.LogServices;

public class MsSqlLoggerService : ILoggerService
{
    private readonly Logger _logger;

    public MsSqlLoggerService(string? connectionString, string? tableName)
    {
        var sinkOpts = new MSSqlServerSinkOptions();
        sinkOpts.AutoCreateSqlTable = true;
        sinkOpts.TableName = tableName;
        
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

        _logger = new LoggerConfiguration()
            .WriteTo
            .MSSqlServer(connectionString: connectionString, sinkOptions: sinkOpts, columnOptions: columnOptions)
            .CreateLogger();
    }
    
    public void Write(string message, EventTypeLevels level, EventType eventType)
    {
        _logger.Information("{message} {Level} {EventTypeName} {EventTypeUid} {Module}",
            message,
            (int)level,
            eventType.EventName,
            eventType.Uid,
            eventType.Module);
    }

    public void Dispose() => _logger.Dispose();
}
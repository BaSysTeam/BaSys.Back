using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.MSSqlServer;

namespace BaSys.Logging.MsSql;

public class MsSqlLoggerService : ILoggerService
{
    private readonly Logger _logger;

    public MsSqlLoggerService(string? connectionString, string? tableName)
    {
        var sinkOpts = new MSSqlServerSinkOptions();
        sinkOpts.AutoCreateSqlTable = true;
        sinkOpts.TableName = tableName;

        _logger = new LoggerConfiguration()
            .WriteTo
            .MSSqlServer(connectionString: connectionString, sinkOptions: sinkOpts)
            .CreateLogger();
    }

    public void Write(string message)
    {
        _logger.Information("Log message is: {message}", message);
    }

    public void Dispose() => _logger.Dispose();
}
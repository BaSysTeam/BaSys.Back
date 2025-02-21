using System.Data;

namespace BaSys.Logging.Abstractions.Abstractions;

public interface ILoggerConfigService
{
    Task<LoggerConfig> GetLoggerConfigAsync();
    Task<LoggerConfig> GetLoggerConfigAsync(IDbConnection connection, IDbTransaction? transaction);
}
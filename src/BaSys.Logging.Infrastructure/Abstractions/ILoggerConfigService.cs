namespace BaSys.Logging.Abstractions.Abstractions;

public interface ILoggerConfigService
{
    Task<LoggerConfig> GetLoggerConfig();
}
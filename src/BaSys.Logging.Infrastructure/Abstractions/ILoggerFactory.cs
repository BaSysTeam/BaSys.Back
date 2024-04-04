namespace BaSys.Logging.Abstractions.Abstractions;

public interface ILoggerFactory
{
    Task<LoggerService> GetLogger();
}
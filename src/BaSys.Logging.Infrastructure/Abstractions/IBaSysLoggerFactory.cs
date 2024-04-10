namespace BaSys.Logging.Abstractions.Abstractions;

public interface IBaSysLoggerFactory
{
    Task<LoggerService> GetLogger();
}
namespace BaSys.Logging.Abstractions.Abstractions;

public interface IBaSysLoggerFactory
{
    Task<ILoggerService> GetLogger();
}
namespace BaSys.Logging.Abstractions.Abstractions;

public interface ILoggerFactory
{
    Task<ILoggerService> GetLogger();
}
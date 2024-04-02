namespace BaSys.Logging.Abstractions.Abstractions;

public interface ILoggerService : IDisposable
{
    void Write(string message);
}
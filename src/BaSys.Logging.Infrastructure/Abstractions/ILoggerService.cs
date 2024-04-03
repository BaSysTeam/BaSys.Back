using BaSys.Logging.Abstractions.Enums;

namespace BaSys.Logging.Abstractions.Abstractions;

public interface ILoggerService : IDisposable
{
    void Write(string message, EventTypeLevels level, EventType eventType);
}
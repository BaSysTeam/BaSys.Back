using BaSys.Logging.Abstractions.Enums;

namespace BaSys.Logging.Abstractions.Abstractions;

public abstract class LoggerService : IDisposable
{
    private readonly EventTypeLevels _minimumLogLevel;
    public LoggerService(LoggerConfig loggerConfig)
    {
        _minimumLogLevel = loggerConfig.MinimumLogLevel;
    }
    
    public void Write(string message, EventTypeLevels level, EventType eventType)
    {
        if (level < _minimumLogLevel)
            return;

        WriteInner(message, level, eventType);
    }
    
    protected abstract void WriteInner(string message, EventTypeLevels level, EventType eventType);
    public abstract void Dispose();
}
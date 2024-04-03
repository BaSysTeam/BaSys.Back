using BaSys.Logging.Abstractions.Enums;
using Serilog.Core;

namespace BaSys.Logging.Abstractions.Abstractions;

public abstract class LoggerService : IDisposable
{
    private readonly EventTypeLevels _minimumLogLevel;
    protected Logger _logger;
    
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
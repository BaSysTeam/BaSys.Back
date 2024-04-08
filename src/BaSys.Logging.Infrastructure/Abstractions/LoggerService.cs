using BaSys.Common.Enums;
using Serilog.Core;

namespace BaSys.Logging.Abstractions.Abstractions;

public abstract class LoggerService : IDisposable
{
    private readonly LoggerConfig _loggerConfig;
    protected Logger? _logger;
    
    public LoggerService(LoggerConfig loggerConfig)
    {
        _loggerConfig = loggerConfig;
    }
    
    public void Write(string message, EventTypeLevels level, EventType eventType)
    {
        if (!_loggerConfig.IsEnabled ||
            level < _loggerConfig.MinimumLogLevel)
            return;

        WriteInner(message, level, eventType);
    }
    
    protected abstract void WriteInner(string message, EventTypeLevels level, EventType eventType);
    public virtual void Dispose() => _logger?.Dispose();
}
using BaSys.Common.Enums;
using Serilog.Core;

namespace BaSys.Logging.Abstractions.Abstractions;

public abstract class LoggerService : IDisposable
{
    private readonly LoggerConfig _loggerConfig;
    
    protected readonly string? _userUid;
    protected readonly string? _userName;
    protected readonly string? _ipAddress;
    
    protected Logger? _logger;
    
    public LoggerService(LoggerConfig loggerConfig, string? userUid, string? userName, string? ipAddress)
    {
        _loggerConfig = loggerConfig;
        _userUid = userUid;
        _userName = userName;
        _ipAddress = ipAddress;
    }
    
    public void Write(string message, EventTypeLevels level, EventType eventType)
    {
        if (!_loggerConfig.IsEnabled ||
            level < _loggerConfig.MinimumLogLevel)
            return;

        WriteInner(message, level, eventType);
    }
    
    protected abstract void WriteInner(string message,EventTypeLevels level, EventType eventType);

    public virtual void Dispose()
    {
        _logger?.Dispose();
    }
}
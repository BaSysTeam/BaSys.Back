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
    
    public void Write(string message,
        EventTypeLevels level,
        EventType eventType,
        Guid? metadataUid = null,
        string? dataUid = null,
        string? dataPresentation = null)
    {
        if (!IsWrite(level))
            return;

        WriteInner(message, level, eventType, null, metadataUid, dataUid, dataPresentation);
    }

    public void Info(string message,
        EventType eventType,
        Guid? metadataUid = null,
        string? dataUid = null,
        string? dataPresentation = null)
    {
        var level = EventTypeLevels.Info;
        if (!IsWrite(level))
            return;
        
        WriteInner(message, level, eventType, null, metadataUid, dataUid, dataPresentation);
    }
    
    public void Error(string exceptionMessage,
        EventType eventType,
        Guid? metadataUid = null,
        string? dataUid = null,
        string? dataPresentation = null)
    {
        var level = EventTypeLevels.Error;
        if (!IsWrite(level))
            return;
        
        WriteInner(null, level, eventType, exceptionMessage, metadataUid, dataUid, dataPresentation);
    }
    
    protected abstract void WriteInner(string? message,
        EventTypeLevels level,
        EventType eventType,
        string? exception = null,
        Guid? metadataUid = null,
        string? dataUid = null,
        string? dataPresentation = null);

    public virtual void Dispose()
    {
        _logger?.Dispose();
    }

    private bool IsWrite(EventTypeLevels level)
    {
        if (!_loggerConfig.IsEnabled ||
            level < _loggerConfig.MinimumLogLevel)
            return false;
        return true;
    }
}
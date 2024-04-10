using BaSys.Common.Enums;

namespace BaSys.Logging.Abstractions;

public class LoggerConfig
{
    public bool IsEnabled { get; set; }
    public LoggerTypes? LoggerType { get; set; }
    public EventTypeLevels MinimumLogLevel { get; set; }
    public string? ConnectionString { get; set; }
    public Guid DbUid { get; set; }
    public string? TableName { get; set; }
    public AutoClearInterval AutoClearInterval { get; set; } = AutoClearInterval.Month;
}
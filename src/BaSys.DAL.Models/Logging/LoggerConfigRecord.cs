using BaSys.Common.Abstractions;
using BaSys.Common.Enums;

namespace BaSys.DAL.Models.Logging;

public class LoggerConfigRecord: SystemObjectBase
{
    public bool IsEnabled { get; set; }
    public LoggerTypes LoggerType { get; set; }
    public EventTypeLevels MinimumLogLevel { get; set; } = EventTypeLevels.Info;
    public string? ConnectionString { get; set; }
    public AutoClearInterval AutoClearInterval { get; set; } = AutoClearInterval.Month;
    public bool IsSelected { get; set; }
}
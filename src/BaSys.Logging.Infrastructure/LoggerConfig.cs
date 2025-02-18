using BaSys.Common.Enums;

namespace BaSys.Logging.Abstractions;

public class LoggerConfig
{
    public bool IsEnabled { get; set; }
    public LoggerTypes? LoggerType { get; set; }
    public EventTypeLevels MinimumLogLevel { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public Guid DbUid { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string WorkflowsLogTableName { get; set; } = string.Empty;
    public AutoClearInterval AutoClearInterval { get; set; } = AutoClearInterval.Month;


    public static string GetTableName(Guid dbUid) => $"logs-{dbUid}";
    public static string GetWorkflowsLogTableName(Guid dbUid) => $"workflows-{dbUid}";
}
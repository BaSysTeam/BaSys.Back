namespace BaSys.Logging.Abstractions;

public class LoggerConfig
{
    public LoggerTypes LoggerType { get; set; }
    public string? ConnectionString { get; set; }
    public string? TableName { get; set; }
}
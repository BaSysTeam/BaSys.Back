using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;

namespace BaSys.Logging.Services;

public class LoggerConfigService : ILoggerConfigService
{
    public async Task<LoggerConfig> GetLoggerConfig()
    {
        return new LoggerConfig
        {
            LoggerType = LoggerTypes.MsSql,
            ConnectionString = "Data Source=OSPC\\SQLEXPRESS19;Initial Catalog=__Serilog;Persist Security Info=True;User ID=sa;Password=QAZwsx!@#;TrustServerCertificate=True;",
            TableName = "Logs"
        };
    } 
}
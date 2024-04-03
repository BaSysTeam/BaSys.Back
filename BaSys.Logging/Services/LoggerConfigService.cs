using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.Abstractions.Enums;

namespace BaSys.Logging.Services;

public class LoggerConfigService : ILoggerConfigService
{
    public async Task<LoggerConfig> GetLoggerConfig()
    {
        var dbUid = new Guid("55dbda0e-4da7-4725-8520-109cb3251f57");
        var tableName = GetTableName(dbUid);
        return new LoggerConfig
        {
            LoggerType = LoggerTypes.MsSql,
            MinimumLogLevel = EventTypeLevels.Error,
            ConnectionString = "Data Source=OSPC\\SQLEXPRESS19;Initial Catalog=__Serilog;Persist Security Info=True;User ID=sa;Password=QAZwsx!@#;TrustServerCertificate=True;",
            DbUid = dbUid,
            TableName = tableName
        };
    }

    private string GetTableName(Guid dbUid) => $"Logs-{dbUid}";
}
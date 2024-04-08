using BaSys.Common.Enums;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.LogServices;

namespace BaSys.Logging.Services;

public class LoggerFactory : ILoggerFactory
{
    private readonly ILoggerConfigService _loggerConfigService;

    public LoggerFactory(ILoggerConfigService loggerConfigService)
    {
        _loggerConfigService = loggerConfigService;
    }
    
    public async Task<LoggerService> GetLogger()
    {
        var loggerConfig = await _loggerConfigService.GetLoggerConfig();

        switch (loggerConfig.LoggerType)
        {
            case LoggerTypes.MsSql:
                return new MsSqlLoggerService(loggerConfig);
            case LoggerTypes.PgSql:
                return new PgSqlLoggerService(loggerConfig);
            case LoggerTypes.MongoDb:
                return new MongoLoggerService(loggerConfig);
            default:
                throw new ApplicationException();
        }
    }
}
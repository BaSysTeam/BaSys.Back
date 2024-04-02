using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.MsSql;

namespace BaSys.Logging.Services;

public class LoggerFactory : ILoggerFactory
{
    private readonly ILoggerConfigService _loggerConfigService;

    public LoggerFactory(ILoggerConfigService loggerConfigService)
    {
        _loggerConfigService = loggerConfigService;
    }
    
    public async Task<ILoggerService> GetLogger()
    {
        var loggerConfig = await _loggerConfigService.GetLoggerConfig();

        switch (loggerConfig.LoggerType)
        {
            case LoggerTypes.MsSql:
                return new MsSqlLoggerService(loggerConfig.ConnectionString, loggerConfig.TableName);
            default:
                throw new ApplicationException();
        }
    }
}
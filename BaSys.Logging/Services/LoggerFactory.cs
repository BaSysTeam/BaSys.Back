using System.Security.Claims;
using BaSys.Common.Enums;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.LogServices;
using Microsoft.AspNetCore.Http;

namespace BaSys.Logging.Services;

public class LoggerFactory : ILoggerFactory
{
    private readonly ILoggerConfigService _loggerConfigService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public LoggerFactory(ILoggerConfigService loggerConfigService, IHttpContextAccessor httpContextAccessor)
    {
        _loggerConfigService = loggerConfigService;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<LoggerService> GetLogger()
    {
        var userUid = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        var userName = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        
        var loggerConfig = await _loggerConfigService.GetLoggerConfig();

        switch (loggerConfig.LoggerType)
        {
            case LoggerTypes.MsSql:
                return new MsSqlLoggerService(loggerConfig, userUid, userName, ipAddress);
            case LoggerTypes.PgSql:
                return new PgSqlLoggerService(loggerConfig, userUid, userName, ipAddress);
            case LoggerTypes.MongoDb:
                return new MongoLoggerService(loggerConfig, userUid, userName, ipAddress);
            default:
                throw new ApplicationException();
        }
    }
}
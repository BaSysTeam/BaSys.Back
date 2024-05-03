using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.Logging;

namespace BaSys.Admin.Abstractions
{
    public interface ILoggerConfigService
    {
        Task<ResultWrapper<LoggerConfig>> GetCurrentLoggerConfigAsync();
        Task<ResultWrapper<LoggerConfig>> GetLoggerConfigByTypeAsync(LoggerTypes loggerType);
        Task<ResultWrapper<int>> CreateLoggerConfigAsync(LoggerConfig loggerConfig);
        Task<ResultWrapper<int>> UpdateLoggerConfigAsync(LoggerConfig loggerConfig);
        Task<ResultWrapper<int>> DeleteLoggerConfigAsync(Guid uid);
    }
}

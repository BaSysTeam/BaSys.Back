using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.Logging;

namespace BaSys.Admin.Abstractions
{
    public interface ILoggerConfigService
    {
        Task<ResultWrapper<LoggerConfigRecord>> GetCurrentLoggerConfigAsync();
        Task<ResultWrapper<LoggerConfigRecord>> GetLoggerConfigByTypeAsync(LoggerTypes loggerType);
        Task<ResultWrapper<int>> CreateLoggerConfigAsync(LoggerConfigRecord loggerConfig);
        Task<ResultWrapper<int>> UpdateLoggerConfigAsync(LoggerConfigRecord loggerConfig);
        Task<ResultWrapper<int>> DeleteLoggerConfigAsync(Guid uid);
    }
}

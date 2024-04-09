using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.Logging;
using BaSys.DTO.Admin;

namespace BaSys.Admin.Abstractions
{
    public interface ILoggerConfigService
    {
        Task<ResultWrapper<LoggerConfigDto>> GetLoggerConfigAsync(string dbName);
        Task<ResultWrapper<int>> CreateLoggerConfigAsync(LoggerConfigDto dto, string dbName);
        Task<ResultWrapper<int>> UpdateLoggerConfigAsync(LoggerConfigDto dto, string dbName);
        Task<ResultWrapper<int>> DeleteLoggerConfigAsync(Guid uid, string dbName);
    }
}

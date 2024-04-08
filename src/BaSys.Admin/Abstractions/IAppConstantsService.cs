using BaSys.Common.Infrastructure;
using BaSys.DTO.Admin;

namespace BaSys.Admin.Abstractions
{
    public interface IAppConstantsService
    {
        Task<ResultWrapper<AppConstantsDto>> GetAppConstantsAsync(string dbName);
        Task<ResultWrapper<int>> CreateAppConstantsAsync(AppConstantsDto dto, string dbName);
        Task<ResultWrapper<int>> UpdateAppConstantsAsync(AppConstantsDto dto, string dbName);
        Task<ResultWrapper<int>> DeleteAppConstantsAsync(Guid uid, string dbName);
    }
}

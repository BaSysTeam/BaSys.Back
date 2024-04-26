using BaSys.Common.Infrastructure;
using BaSys.DTO.Admin;

namespace BaSys.Admin.Abstractions
{
    public interface IAppConstantsService
    {
        Task<ResultWrapper<AppConstantsDto>> GetAppConstantsAsync();
        Task<ResultWrapper<int>> CreateAppConstantsAsync(AppConstantsDto dto);
        Task<ResultWrapper<int>> UpdateAppConstantsAsync(AppConstantsDto dto);
        Task<ResultWrapper<int>> DeleteAppConstantsAsync(Guid uid);
    }
}

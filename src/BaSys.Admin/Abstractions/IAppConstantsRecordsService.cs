using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using BaSys.Common.Models;

namespace BaSys.Admin.Abstractions
{
    public interface IAppConstantsRecordsService
    {
        Task<ResultWrapper<AppConstantsRecordDto>> GetAppConstantsRecordAsync(string dbName);
        Task<ResultWrapper<int>> CreateAppConstantsRecordAsync(AppConstantsRecordDto appConstant, string dbName);
        Task<ResultWrapper<int>> UpdateAppConstantsRecordAsync(AppConstantsRecordDto appConstant, string dbName);
        Task<ResultWrapper<int>> DeleteAppConstantsRecordAsync(Guid uid, string dbName);
    }
}

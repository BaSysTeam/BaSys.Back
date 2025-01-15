using BaSys.Common.Infrastructure;
using BaSys.DTO.Constructor;
using BaSys.Metadata.Models.MenuModel;
using System.Data;

namespace BaSys.Core.Abstractions
{
    public interface IMetaWorkflowsService
    {
        void SetUp(IDbConnection connection);
        Task<ResultWrapper<MetaObjectListDto>> GetListAsync();
        Task<ResultWrapper<WorkflowSettingsDto>> GetSettingsItemAsync(string objectName);
        Task<ResultWrapper<int>> CreateAsync(WorkflowSettingsDto settings);
        Task<ResultWrapper<int>> UpdateSettingsItemAsync(WorkflowSettingsDto settings);
        Task<ResultWrapper<int>> DeleteAsync(string objectName);
    }
}

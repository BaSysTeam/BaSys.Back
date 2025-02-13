using BaSys.Common.Infrastructure;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Workflows.DTO;
using System.Data;

namespace BaSys.Workflows.Abstractions
{
    public interface IWorkflowTriggersService
    {
        void SetUp(IDbConnection connection);
        Task<ResultWrapper<IEnumerable<WorkflowTriggerDto>>> GetCollectionAsync(Guid? metaObjectUid, Guid? workflowUid);
        Task<ResultWrapper<WorkflowTrigger>> GetItemAsync(Guid uid);
        Task<ResultWrapper<Guid>> CreateAsync(WorkflowTrigger itme);
        Task<ResultWrapper<int>> UpdateAsync(WorkflowTrigger item);
        Task<ResultWrapper<int>> DeleteAsync(Guid uid);
    }
}

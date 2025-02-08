using BaSys.Common.Infrastructure;
using BaSys.Metadata.Models.WorkflowModel;
using System.Data;

namespace BaSys.Workflows.Abstractions
{
    public interface IWorkflowsScheduleService
    {
        void SetUp(IDbConnection connection);
        Task<ResultWrapper<WorkflowScheduleRecord>> GetRecordAsync(Guid uid);
        Task<ResultWrapper<IEnumerable<WorkflowScheduleRecord>>> GetCollectionAsync(Guid? workflowUid, bool? isActive);
        Task<ResultWrapper<Guid>> CreateAsync(WorkflowScheduleRecord record);
        Task<ResultWrapper<int>> UpdateAsync(WorkflowScheduleRecord record);
        Task<ResultWrapper<int>> DeleteAsync(Guid uid);
    }
}

using BaSys.Common.Infrastructure;
using BaSys.Logging.Workflow;
using System.Data;

namespace BaSys.Constructor.Abstractions
{
    public interface IWorkflowLogRecordsService
    {
        Task<ResultWrapper<IEnumerable<WorkflowLogRecord>>> GetLifecycleRecordsAsync(string workflowUid);
        Task<ResultWrapper<IEnumerable<WorkflowLogRecord>>> GetRecordsByRunAsync(string runUid);
        Task<ResultWrapper<int>> DeleteWorkflowRecordsAsync(string workflowUid);
    }
}

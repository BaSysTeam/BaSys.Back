using BaSys.Common.Infrastructure;
using BaSys.Workflows.DTO;

namespace BaSys.Admin.Abstractions
{
    public interface IWorkflowsBoardService
    {
        Task<ResultWrapper<IEnumerable<WorkflowInfoDto>>> GetInfoAsync();
    }
}

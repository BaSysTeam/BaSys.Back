using BaSys.Common.Infrastructure;

namespace BaSys.Workflows.Abstractions
{
    public interface IWorkflowTerminateCommandHandler
    {
        Task<ResultWrapper<bool>> ExecuteAsync(string runUid);
    }
}
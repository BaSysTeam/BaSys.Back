using BaSys.Common.Infrastructure;
using BaSys.DTO.Constructor;
using BaSys.Workflows.DTO;
using System.Data;

namespace BaSys.Core.Abstractions
{
    public interface IWorkflowsService
    {
        void SetUp(IDbConnection connection);
        Task<ResultWrapper<WorkflowStartDto>> StartAsync(string name);
        Task<ResultWrapper<WorkflowCheckDto>> CheckAsync(string reference);
        Task<ResultWrapper<bool>> TerminateAsync(string reference);
    }
}

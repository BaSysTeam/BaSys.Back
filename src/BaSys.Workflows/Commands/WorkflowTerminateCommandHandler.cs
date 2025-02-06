using BaSys.Common.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Workflows.Abstractions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Commands
{
    public sealed class WorkflowTerminateCommandHandler : ICommandHandlerBase<string, bool>, IWorkflowTerminateCommandHandler
    {
        private readonly IWorkflowHost _host;

        public WorkflowTerminateCommandHandler(IWorkflowHost host)
        {
            _host = host;
        }

        public async Task<ResultWrapper<bool>> ExecuteAsync(string runUid)
        {
            var result = new ResultWrapper<bool>();

            try
            {
                var workflow = await _host.PersistenceStore.GetWorkflowInstance(runUid);
                if (workflow == null)
                {
                    result.Error(-1, $"Workflow {runUid} not found.");
                    return result;
                }

                if (workflow.Status == WorkflowStatus.Runnable)
                {
                    var isTerminated = await _host.TerminateWorkflow(runUid);
                    if (isTerminated)
                    {
                        result.Success(isTerminated, $"Workflow termitated.");
                    }
                    else
                    {
                        result.Error(-1, $"Workflow was not terminated.", $"Run Uid: {runUid}.");
                    }
                    
                }
                else
                {
                    result.Error(-1, $"Cannot terminate workflow {runUid}, current status: {workflow.Status}");
                }

            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot terminate workflow {runUid}: {ex.Message}", ex.StackTrace);
            }

            return result;
        }
    }
}

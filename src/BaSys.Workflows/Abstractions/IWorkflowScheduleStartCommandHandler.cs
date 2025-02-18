using BaSys.Common.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Workflows.Commands;

namespace BaSys.Workflows.Abstractions
{
    public interface IWorkflowScheduleStartCommandHandler: ICommandHandlerBase<WorkflowScheduleStartCommand, bool>
    {
        new Task<ResultWrapper<bool>> ExecuteAsync(WorkflowScheduleStartCommand command);
    }
}

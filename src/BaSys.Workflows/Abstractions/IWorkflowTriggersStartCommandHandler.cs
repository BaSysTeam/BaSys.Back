using BaSys.Common.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Workflows.Commands;

namespace BaSys.Workflows.Abstractions
{
    public interface IWorkflowTriggersStartCommandHandler: ICommandHandlerBase<WorkflowTriggersStartCommand, bool>
    {
       new Task<ResultWrapper<bool>> ExecuteAsync(WorkflowTriggersStartCommand command);
    }
}

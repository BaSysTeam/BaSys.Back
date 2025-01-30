using BaSys.Common.Enums;
using BaSys.Logging.InMemory;
using BaSys.Workflows.Infrastructure;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Steps
{
    public abstract class StepAsyncBase : StepBodyAsync
    {
        protected InMemoryLogger? _logger;

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            Init(context);

            return await ExecuteAsync(context);
        }

        public abstract Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context);

        protected virtual void Init(IStepExecutionContext context)
        {
            var workflowData = context.Workflow.Data as BaSysWorkflowData;
            if (workflowData == null)
            {
                throw new ArgumentNullException($"Cannot cast workflow data in step {nameof(MessageStep)}");
            }

            _logger = new InMemoryLogger(EventTypeLevels.Info, workflowData.Log);

        }

        protected void LogInfo(string message)
        {
            _logger?.Log(EventTypeLevels.Info, message);
        }

        protected void LogError(string message)
        {
            _logger?.Log(EventTypeLevels.Error, message);
        }


    }
}

using BaSys.Common.Enums;
using BaSys.Logging.InMemory;
using BaSys.Logging.Workflow;
using BaSys.Logging.Workflow.Abstractions;
using BaSys.Workflows.Infrastructure;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Steps
{
    public abstract class StepAsyncBase : StepBodyAsync
    {
        protected InMemoryLogger? _inMemoryLogger;
        protected IWorkflowLogger? _logger;

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

            _inMemoryLogger = new InMemoryLogger(EventTypeLevels.Info, workflowData.Log);

            var loggerFactory = new WorkflowLoggerFactory(workflowData.LoggerConfig, workflowData.LoggerContext);
            _logger = loggerFactory.GetLogger();

        }

        protected void LogInfo(string message)
        {
            _inMemoryLogger?.Log(EventTypeLevels.Info, message);
            _logger?.Info(WorkflowLogEventKinds.Regular, this.GetType().Name, message);
        }

        protected void LogError(string message)
        {
            _inMemoryLogger?.Log(EventTypeLevels.Error, message);
            _logger?.Error(WorkflowLogEventKinds.Regular, this.GetType().Name, message);
        }


    }
}

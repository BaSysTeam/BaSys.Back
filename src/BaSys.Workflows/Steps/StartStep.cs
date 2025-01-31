using BaSys.Workflows.Infrastructure;
using System.Text.Json;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Steps
{
    public sealed class StartStep : StepAsyncBase
    {
        public override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
        {
            LogStart();

            if (context.Workflow.Data is BaSysWorkflowData workflowData)
            {
                var json = string.Empty;
                if (workflowData.Parameters.Any())
                {
                    json = JsonSerializer.Serialize(workflowData.Parameters, new JsonSerializerOptions { WriteIndented = true });
                }

                LogInfo($"parameters: {json}");
            }

            return Task.FromResult(ExecutionResult.Next());
        }

        private void LogStart()
        {
            var message = "Workflow started";
            _inMemoryLogger?.LogInfo(message);
            _logger?.Info(Common.Enums.WorkflowLogEventKinds.Start, this.GetType().Name, message);
        }
    }
}

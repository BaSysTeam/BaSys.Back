using BaSys.Workflows.Infrastructure;
using System.Text.Json;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Steps
{
    public sealed class StartStep: StepAsyncBase
    {
        public override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
        {
            LogInfo("Workflow started");

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
    }
}

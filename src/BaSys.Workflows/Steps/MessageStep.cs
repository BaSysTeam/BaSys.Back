using BaSys.Common.Enums;
using BaSys.Logging.InMemory;
using System.Diagnostics;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Steps
{
    public class MessageStep : StepAsyncBase
    {
        public string Message { get; set; } = string.Empty; 

        public override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
        {
          
            LogInfo(Message);

            return Task.FromResult( ExecutionResult.Next());
        }
    }
}

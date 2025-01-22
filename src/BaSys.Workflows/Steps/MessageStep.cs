using System.Diagnostics;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Steps
{
    public class MessageStep : StepBody
    {
        public string Message { get; set; } = string.Empty;

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Debug.WriteLine($"Message: {Message}");
            Console.WriteLine($"Message: {Message}");
            return ExecutionResult.Next();
        }
    }
}

using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Steps
{
    public class MessageStep : StepBody
    {
        public string Message { get; set; } = string.Empty;

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine($"Message: {Message}");
            return ExecutionResult.Next();
        }
    }
}

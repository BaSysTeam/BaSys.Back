using System.Diagnostics;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Steps
{
    public class DummyStep: StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Debug.WriteLine($"Run Dummy at: {DateTime.UtcNow}");
            Console.WriteLine($"Run Dummy at: {DateTime.UtcNow}");
            return ExecutionResult.Next();
        }
    }
}

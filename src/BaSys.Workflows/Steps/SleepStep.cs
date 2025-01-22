using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Steps
{
    public class SleepStep : StepBodyAsync
    {
        public int Delay { get; set; }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            Console.WriteLine($"Sleep for {Delay} ms..........");
            await Task.Delay(Delay);  // Simulate async work
            Console.WriteLine($"Awake from sleep");

            return ExecutionResult.Next();
        }
    }
}

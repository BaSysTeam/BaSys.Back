using System.Diagnostics;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Steps
{
    public class SleepStep : StepBodyAsync
    {
        public string Delay { get; set; } = string.Empty;

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            Console.WriteLine($"Sleep for {Delay} ms at {DateTime.UtcNow}..........");

            int.TryParse(Delay, out var delayValue);

            await Task.Delay(delayValue);  // Simulate async work
            Console.WriteLine($"Awake from sleep at {DateTime.UtcNow}");

            return ExecutionResult.Next();
        }
    }
}

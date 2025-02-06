using System.Diagnostics;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Steps
{
    public class SleepStep : StepAsyncBase
    {
        public string Delay { get; set; } = string.Empty;

        public override async Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
        {
            LogInfo($"Sleep for {Delay} ms..........");

            int.TryParse(Delay, out var delayValue);

            //await Task.Delay(delayValue);  // Simulate async work
            //Console.WriteLine($"Awake from sleep at {DateTime.UtcNow}");

            int interval = 1000;  // Check every 1 second
            int elapsed = 0;

            while (elapsed < delayValue)
            {
                // Console.WriteLine($"{DateTime.UtcNow}. Cancellation token: {context.CancellationToken}, CancelRequested: {context.CancellationToken.IsCancellationRequested}");

                if (context.CancellationToken.IsCancellationRequested)
                {
                    LogInfo($"Sleep was canceled at {DateTime.UtcNow}");
                    return ExecutionResult.Persist(context.PersistenceData);
                }

                try
                {
                    // Monitor the cancellation token from the context
                    await Task.Delay(interval, context.CancellationToken);

                }
                catch (TaskCanceledException)
                {
                    LogInfo($"Sleep was canceled at {DateTime.UtcNow}");
                    return ExecutionResult.Persist(context.PersistenceData);
                }

                elapsed += interval;
            }

            LogInfo($"Awake from sleep");

            return ExecutionResult.Next();
        }
    }
}

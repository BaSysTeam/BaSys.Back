using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.Workflows.Abstractions;
using BaSys.Workflows.Commands;
using BaSys.Workflows.Infrastructure;
using Microsoft.CodeAnalysis;

namespace BaSys.Host.Services
{
    public class WorkflowsSchedulerService : BackgroundService
    {
        private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
        private readonly IMainConnectionFactory _connectionFactory;
        private readonly IWorkflowScheduleStartCommandHandler _startHandler;

        public WorkflowsSchedulerService(IMainConnectionFactory connectionFactory,
            IDbInfoRecordsProvider dbInfoRecordsProvider,
            IWorkflowScheduleStartCommandHandler startHandler)
        {
            _connectionFactory = connectionFactory;
            _dbInfoRecordsProvider = dbInfoRecordsProvider;
            _startHandler = startHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _dbInfoRecordsProvider.Update();

            var dbInfoRecords = _dbInfoRecordsProvider.GetActiveRecords();
            Console.WriteLine($"WorkflowsScheduler. Found active db info records: {dbInfoRecords.Count}");

            while (!stoppingToken.IsCancellationRequested)
            {

                var now = DateTime.UtcNow;
                var nextRun = now.AddMinutes(1).AddSeconds(-now.Second);
                var delay = nextRun - now;

                Console.WriteLine($"WorkflowsScheduler. Next execution at {nextRun} (delay {delay.TotalSeconds} seconds)");

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                Console.WriteLine($"WorkflowsScheduler. Check workflows Schedule at {DateTime.UtcNow}");


                foreach (var record in dbInfoRecords)
                {
                    using (var connection = _connectionFactory.CreateConnection(record.ConnectionString, record.DbKind))
                    {
                        var provider = new WorkflowScheduleProvider(connection);
                        var constantsProvider = new AppConstantsProvider(connection);
                        var recordsToStart = new List<WorkflowScheduleRecord>();
                        try
                        {
                            var appConstants = await constantsProvider.GetConstantsAsync(null);

                            if (!appConstants?.UseWorkflowsScheduler ?? false)
                            {
                                Console.WriteLine($"WorkflowScheduler. Scheduler disabled for base {record.Name}.");
                                continue;
                            }

                            var scheduleRecords = await provider.GetCollectionAsync(null, true, null);
                            foreach (var scheduleRecord in scheduleRecords)
                            {
                                if (CronHelper.CronMatches(scheduleRecord.CronExpression, DateTime.UtcNow))
                                {
                                    recordsToStart.Add(scheduleRecord);
                                }
                            }

                            Console.WriteLine($"WorkflowsScheduler. Found {scheduleRecords.Count()} active records in DB {record.Name}.");
                            Console.WriteLine($"WorkflowsScheduler. Records to start: {recordsToStart.Count}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"WorkflowScheduler. Cannot get workflow schedule records: {ex.Message} {ex.StackTrace}");
                        }

                        if (recordsToStart.Any())
                        {
                            var startCommand = new WorkflowScheduleStartCommand();
                            startCommand.ScheduleRecords = recordsToStart;
                            startCommand.DbInfoRecord = record;

                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await _startHandler.ExecuteAsync(startCommand);

                                }
                                catch (Exception ex)
                                {
                                    // Cannot start triggers.
                                    var message = $"WorkflowScheduler. Cannot start workflows: {ex.Message}, {ex.StackTrace}";
                                    Console.WriteLine(message);

                                }
                            });

                        }

                    }
                }

            }
        }

    }
}

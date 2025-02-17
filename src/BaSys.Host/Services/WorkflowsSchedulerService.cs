using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.Workflows.Infrastructure;

namespace BaSys.Host.Services
{
    public class WorkflowsSchedulerService : BackgroundService
    {
        private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
        private readonly IMainConnectionFactory _connectionFactory;

        public WorkflowsSchedulerService(IMainConnectionFactory connectionFactory, 
            IDbInfoRecordsProvider dbInfoRecordsProvider)
        {
            _connectionFactory = connectionFactory;
            _dbInfoRecordsProvider = dbInfoRecordsProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _dbInfoRecordsProvider.Update();

            var dbInfoRecords = _dbInfoRecordsProvider.GetActiveRecords();
            Console.WriteLine($"WorkflowsScheduler. Found active db info records: {dbInfoRecords.Count}");

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"WorkflowsScheduler. Check workflows Schedule at {DateTime.UtcNow}");


                foreach (var record in dbInfoRecords)
                {
                    using(var connection = _connectionFactory.CreateConnection(record.ConnectionString, record.DbKind))
                    {
                        var provider = new WorkflowScheduleProvider(connection);
                        var recordsToStart = new List<WorkflowScheduleRecord>();
                        try
                        {
                            var scheduleRecords = await provider.GetCollectionAsync(null, true, null);
                            foreach(var scheduleRecord in scheduleRecords)
                            {
                               if (CronHelper.CronMatches(scheduleRecord.CronExpression, DateTime.UtcNow))
                                {
                                    recordsToStart.Add(scheduleRecord);
                                }
                            }

                            Console.WriteLine($"WorkflowsScheduler. Found {scheduleRecords.Count()} active records in DB {record.Name}.");
                            Console.WriteLine($"WorkflowsScheduler. Records to start: {recordsToStart.Count}");
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine($"WorkflowScheduler. Cannot get workflow schedule records: {ex.Message} {ex.StackTrace}");
                        }
                        
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

       
    }
}

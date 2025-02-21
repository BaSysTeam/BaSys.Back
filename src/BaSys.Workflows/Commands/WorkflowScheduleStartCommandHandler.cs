using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.Workflow;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Workflows.Abstractions;
using BaSys.Workflows.Infrastructure;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Commands
{
    public sealed class WorkflowScheduleStartCommandHandler : IWorkflowScheduleStartCommandHandler
    {
        private readonly IMainConnectionFactory _connectionFactory;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly ILoggerConfigService _loggerConfigService;
        private readonly IWorkflowHost _host;
        private readonly IWorkflowRegistry _wRegistry;

        public WorkflowScheduleStartCommandHandler(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            ILoggerConfigService loggerConfigService,
            IWorkflowHost host,
            IDefinitionLoader definitionLoader,
            IWorkflowRegistry wRegistry)
        {
            _connectionFactory = connectionFactory;
            _providerFactory = providerFactory;
            _loggerConfigService = loggerConfigService;

            _host = host;
            _wRegistry = wRegistry;
        }

        public async Task<ResultWrapper<bool>> ExecuteAsync(WorkflowScheduleStartCommand command)
        {
            var result = new ResultWrapper<bool>();

            using (var connection = _connectionFactory.CreateConnection(command.DbInfoRecord.ConnectionString, command.DbInfoRecord.DbKind))
            {
                _providerFactory.SetUp(connection);
                var provider = _providerFactory.Create<MetaWorkflowsProvider>();

                var loggerConfig = await _loggerConfigService.GetLoggerConfigAsync(connection, null);

                foreach (var record in command.ScheduleRecords)
                {
                    await StartTriggerAsync(provider, command,
                        record, loggerConfig);
                }
            }

            result.Success(true);
            return result;
        }

        private async Task<string> StartTriggerAsync(
            MetaWorkflowsProvider provider,
            WorkflowScheduleStartCommand command,
            WorkflowScheduleRecord record, 
            LoggerConfig loggerConfig)
        {
            var metaWorkflow = await provider.GetItemAsync(record.WorkflowUid, null);

            if (!record.IsActive)
            {
                return string.Empty;
            }

            if (metaWorkflow == null)
            {
                return string.Empty;
            }

            var workflowSettings = metaWorkflow.ToSettings();

            var isRegistered = _wRegistry.IsRegistered(workflowSettings.Name, (int)workflowSettings.Version);

            WorkflowDefinition workflowDefinition;
            if (!isRegistered)
            {
                // Build the workflow definition
                workflowDefinition = WorkflowBuilder.Build(workflowSettings);

                // Register the workflow definition dynamically
                _wRegistry.RegisterWorkflow(workflowDefinition);
            }
            else
            {
                workflowDefinition = _wRegistry.GetDefinition(workflowSettings.Name);
            }

            // Start the workflow
            var loggerContext = new WorkflowLoggerContext();
            loggerContext.WorkflowUid = workflowSettings.Uid;
            loggerContext.WorkflowName = workflowSettings.Name;
            loggerContext.WorkflowTitle = workflowSettings.Title;
            loggerContext.Version = workflowSettings.Version;
            loggerContext.UserUid = string.Empty;
            loggerContext.UserName = "system";
            loggerContext.Origin = "scheduler";

            var workflowData = new BaSysWorkflowData();
            workflowData.LoggerConfig = loggerConfig;
            workflowData.LoggerContext = loggerContext;

            string runUid = await _host.StartWorkflow(workflowSettings.Name, workflowData, Guid.NewGuid().ToString());
            loggerContext.RunUid = runUid;

            return runUid;
        }
    }
}

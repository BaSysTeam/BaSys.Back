using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Workflow;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Workflows.Abstractions;
using BaSys.Workflows.Infrastructure;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Commands
{
    public sealed class WorkflowTriggersStartCommandHandler : IWorkflowTriggersStartCommandHandler
    {
        private readonly IMainConnectionFactory _connectionFactory; 
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly IWorkflowHost _host;
        private readonly IWorkflowRegistry _wRegistry;

        public WorkflowTriggersStartCommandHandler(IMainConnectionFactory connectionFactory, 
            ISystemObjectProviderFactory providerFactory,
            IWorkflowHost host,
            IDefinitionLoader definitionLoader,
            IWorkflowRegistry wRegistry)
        {

            _connectionFactory = connectionFactory;
            _providerFactory = providerFactory;

            _host = host;
            _wRegistry = wRegistry;
        }

        public async Task<ResultWrapper<bool>> ExecuteAsync(WorkflowTriggersStartCommand command)
        {
            var result = new ResultWrapper<bool>();

            if (command.DbInfoRecord == null)
            {
                throw new ArgumentNullException($"Cannot start triggers. DbInfoRecord is empty.");
            }

            using (var connection = _connectionFactory.CreateConnection(command.DbInfoRecord.ConnectionString, command.DbInfoRecord.DbKind))
            {
                _providerFactory.SetUp(connection);
                var provider = _providerFactory.Create<MetaWorkflowsProvider>();

                foreach (var trigger in command.Triggers)
                {
                    await StartTriggerAsync(provider, command,
                        trigger);
                }
            }

            result.Success(true);
            return result;

        }

        private async Task<string> StartTriggerAsync(MetaWorkflowsProvider provider, WorkflowTriggersStartCommand command, 
            WorkflowTrigger trigger)
        {
            var metaWorkflow = await provider.GetItemAsync(trigger.WorkflowUid, null);

            if (metaWorkflow == null)
            {
                throw new ArgumentNullException($"Cannot find workflow: {trigger.WorkflowUid}");
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
            loggerContext.UserUid = command.UserUid;
            loggerContext.UserName = command.UserName;
            loggerContext.Origin = "trigger";

            var workflowData = new BaSysWorkflowData();
            workflowData.SetParameters(command.Parameters);
            workflowData.LoggerConfig = command.LoggerConfig;
            workflowData.LoggerContext = loggerContext;

            string runUid = await _host.StartWorkflow(workflowSettings.Name, workflowData, Guid.NewGuid().ToString());
            loggerContext.RunUid = runUid;

            return runUid;
        }
       
    }
}

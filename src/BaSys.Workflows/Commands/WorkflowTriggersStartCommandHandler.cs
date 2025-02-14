using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.Workflow;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Workflows.Abstractions;
using BaSys.Workflows.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Security.Claims;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Workflows.Commands
{
    public sealed class WorkflowTriggersStartCommandHandler : IWorkflowTriggersStartCommandHandler, IDisposable
    {
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly IWorkflowHost _host;
        private readonly IWorkflowRegistry _wRegistry;

        private readonly IDbConnection _connection;
        private readonly MetaWorkflowsProvider _provider;
        private bool _disposed;

        public WorkflowTriggersStartCommandHandler(IMainConnectionFactory connectionFactory, 
            ISystemObjectProviderFactory providerFactory,
            IWorkflowHost host,
            IDefinitionLoader definitionLoader,
            IWorkflowRegistry wRegistry)
        {

            _connection = connectionFactory.CreateConnection();

            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _provider = _providerFactory.Create<MetaWorkflowsProvider>();

            _host = host;
            _wRegistry = wRegistry;
        }

        public async Task<ResultWrapper<bool>> ExecuteAsync(WorkflowTriggersStartCommand command)
        {
            var result = new ResultWrapper<bool>();

         

            foreach (var trigger in command.Triggers)
            {
                await StartTriggerAsync(command, 
                    trigger);
            }

            return result;

        }

        private async Task<string> StartTriggerAsync(WorkflowTriggersStartCommand command, 
            WorkflowTrigger trigger)
        {
            var metaWorkflow = await _provider.GetItemAsync(trigger.WorkflowUid, null);

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

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connection != null)
                        _connection.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

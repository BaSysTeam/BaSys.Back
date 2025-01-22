using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Metadata.Models.WorkflowModel.Steps;
using BaSys.Workflows.Steps;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Services;
using static Dapper.SqlMapper;

namespace BaSys.Core.Services
{
    public class WorkflowsService : IWorkflowsService
    {
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly ILoggerService _logger;
        private readonly IWorkflowHost _host;
        private readonly IWorkflowRegistry _wRegistry;

        private IDbConnection _connection;
        private MetaWorkflowsProvider _provider;

        public WorkflowsService(ISystemObjectProviderFactory providerFactory,
            ILoggerService logger,
            IWorkflowHost host,
            IDefinitionLoader definitionLoader, 
            IWorkflowRegistry wRegistry)
        {
            _providerFactory = providerFactory;
            _host = host;
            _wRegistry = wRegistry;

            _logger = logger;
        }

        public void SetUp(IDbConnection connection)
        {
            _connection = connection;

            _providerFactory.SetUp(_connection);
            _provider = _providerFactory.Create<MetaWorkflowsProvider>();
        }

        public async Task<ResultWrapper<string>> StartAsync(string name)
        {
            var result = new ResultWrapper<string>();

            try
            {
                var metaWorkflow = await _provider.GetItemByNameAsync(name, null);

                if (metaWorkflow == null)
                {
                    result.Error(-1, $"Cannot find workflow: {name}");
                    return result;
                }

                var workflowSettings = metaWorkflow.ToSettings();

                var isRegistered = _wRegistry.IsRegistered(workflowSettings.Name, (int)workflowSettings.Version);

                if (!isRegistered)
                {
                    // Build the workflow definition
                    var workflowDefinition = BuildWorkflow(workflowSettings);

                    // Register the workflow definition dynamically
                    _wRegistry.RegisterWorkflow(workflowDefinition);
                }

                // Start the workflow
                var runUid = Guid.NewGuid().ToString();
                string workflowId = await _host.StartWorkflow(workflowSettings.Name, null, runUid);


                result.Success(runUid, $"Workflow \"{name}\" started");
            }
            catch(Exception ex)
            {
                result.Error(-1, $"Cannot start workflow: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        private WorkflowDefinition BuildWorkflow(WorkflowSettings settings)
        {

            var workflowDefinition = new WorkflowDefinition()
            {
                Id = settings.Name,
                Version = (int)settings.Version,
                DataType = typeof(object)
            };

            var stepId = 0;
            var identifiersIndex = new Dictionary<Guid, int>();

            // Create steps.
            foreach (var stepSettings in settings.Steps)
            {
                if (stepSettings is MessageStepSettings messageStepSettings)
                {
                    var messageStep = new WorkflowStep<MessageStep>();
                    messageStep.Id = stepId;
                    messageStep.Name = stepSettings.Name;

                    messageStep.Inputs.Add(
                        new MemberMapParameter(
                        (Expression<Func<MessageStepSettings, object>>)(data => data.Message),
                        (Expression<Func<MessageStep, object>>)(step => step.Message))
                        );

                    workflowDefinition.Steps.Add(messageStep);
                }
                else if (stepSettings is SleepStepSettings sleepStepSettings)
                {
                    var sleepStep = new WorkflowStep<SleepStep>();
                    sleepStep.Id = stepId;
                    sleepStep.Name = stepSettings.Name;

                    sleepStep.Inputs.Add(
                      new MemberMapParameter(
                      (Expression<Func<SleepStepSettings, object>>)(data => data.Delay),
                      (Expression<Func<SleepStep, object>>)(step => step.Delay))
                      );

                    workflowDefinition.Steps.Add(sleepStep);
                }

                identifiersIndex.Add(stepSettings.Uid, stepId);

                stepId++;
            }

            // Define step outcomes.
            foreach (var stepSettings in settings.Steps)
            {
                if (stepSettings.PreviousStepUid.HasValue)
                {
                    var currentStepId = identifiersIndex[stepSettings.Uid];
                    var previousStepId = identifiersIndex[stepSettings.PreviousStepUid.Value];

                    var previousStep = workflowDefinition.Steps.FindById(previousStepId);

                    if (previousStep != null)
                    {
                        previousStep.Outcomes.Add(new ValueOutcome
                        {
                            NextStep = currentStepId
                        });
                    }
                }
            }

            return workflowDefinition;
        }
    }
}

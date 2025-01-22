using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Metadata.Models.WorkflowModel.Steps;
using BaSys.Workflows.DTO;
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
        private readonly IPersistenceProvider _persistenceProvider;

        private IDbConnection _connection;
        private MetaWorkflowsProvider _provider;

        public WorkflowsService(ISystemObjectProviderFactory providerFactory,
            ILoggerService logger,
            IWorkflowHost host,
            IDefinitionLoader definitionLoader,
            IWorkflowRegistry wRegistry,
            IPersistenceProvider persistenceProvider)
        {
            _providerFactory = providerFactory;
            _host = host;
            _wRegistry = wRegistry;
            _persistenceProvider = persistenceProvider;

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
                string runUid = await _host.StartWorkflow(workflowSettings.Name, null, Guid.NewGuid().ToString());

                result.Success(runUid, $"Workflow \"{name}\" started");
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot start workflow: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<WorkflowCheckDto>> CheckAsync(string runUid)
        {
            var result = new ResultWrapper<WorkflowCheckDto>();

            try
            {
                var instanse = await _host.PersistenceStore.GetWorkflowInstance(runUid);

                var dto = new WorkflowCheckDto()
                {
                    Status = instanse.Status,
                    Id = instanse.Id,
                    Reference = instanse.Reference
                };

                result.Success(dto);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get Workflow status by reference {runUid}: {ex.Message}", ex.StackTrace);
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
                    messageStep.Name = typeof(MessageStep).Name;

                    var messageParameter = new MemberMapParameter(
                        (Expression<Func<object, string>>)(data => messageStepSettings.Message),
                        (Expression<Func<MessageStep, string>>)(step => step.Message));

                    messageStep.Inputs.Add(messageParameter);

                    workflowDefinition.Steps.Add(messageStep);

                }
                else if (stepSettings is SleepStepSettings sleepStepSettings)
                {
                    var sleepStep = new WorkflowStep<SleepStep>();
                    sleepStep.Id = stepId;
                    sleepStep.Name = typeof(SleepStep).Name;

                    sleepStep.Inputs.Add(
                      new MemberMapParameter(
                      (Expression<Func<object, string>>)(data => sleepStepSettings.Delay),
                      (Expression<Func<SleepStep, string>>)(step => step.Delay))
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

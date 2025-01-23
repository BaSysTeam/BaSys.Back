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

        public async Task<ResultWrapper<WorkflowStartDto>> StartAsync(string name)
        {
            var result = new ResultWrapper<WorkflowStartDto>();

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

                WorkflowDefinition workflowDefinition;
                if (!isRegistered)
                {
                    // Build the workflow definition
                    workflowDefinition = BuildWorkflow(workflowSettings);

                    // Register the workflow definition dynamically
                    _wRegistry.RegisterWorkflow(workflowDefinition);
                }
                else
                {
                    workflowDefinition = _wRegistry.GetDefinition(workflowSettings.Name);
                }

                var startDto = new WorkflowStartDto();
                foreach(var step in workflowDefinition.Steps)
                {
                    var stepDto = new WorkflowStepDto();
                    stepDto.Id = step.Id;
                    stepDto.Name = step.Name;

                    if (!string.IsNullOrEmpty(step.ExternalId))
                    {
                        if (Guid.TryParse(step.ExternalId, out var uid))
                        {
                            var stepSettings = workflowSettings.Steps.FirstOrDefault(x => x.Uid == uid);
                            stepDto.Title = stepSettings?.Title ?? string.Empty;
                        }
                    }

                    startDto.Steps.Add(stepDto);
                   
                }

                // Start the workflow
                string runUid = await _host.StartWorkflow(workflowSettings.Name, null, Guid.NewGuid().ToString());
                startDto.RunUid = runUid;

                result.Success(startDto, $"Workflow \"{name}\" started");
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

        public async Task<ResultWrapper<bool>> TerminateAsync(string runUid)
        {
            var result = new ResultWrapper<bool>();

            try
            {
                var workflow = await _host.PersistenceStore.GetWorkflowInstance(runUid);
                if (workflow == null)
                {
                    result.Error(-1, $"Workflow {runUid} not found.");
                    return result;
                }

                if (workflow.Status == WorkflowStatus.Runnable)
                {
                    var isTerminated = await _host.TerminateWorkflow(runUid);
                    result.Success(isTerminated);
                }
                else
                {
                    result.Error(-1, $"Cannot terminate workflow {runUid}, current status: {workflow.Status}");
                }

            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot terminate workflow {runUid}: {ex.Message}", ex.StackTrace);
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
                    messageStep.ExternalId = messageStepSettings.Uid.ToString();

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
                    sleepStep.ExternalId = sleepStepSettings.Uid.ToString();

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

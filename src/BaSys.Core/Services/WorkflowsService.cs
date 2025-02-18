using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.Workflow;
using BaSys.Translation;
using BaSys.Workflows.DTO;
using BaSys.Workflows.Infrastructure;
using System.Data;
using System.Security.Claims;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Core.Services
{
    public class WorkflowsService : IWorkflowsService
    {
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly IWorkflowHost _host;
        private readonly IWorkflowRegistry _wRegistry;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerConfigService _loggerConfigService;

        private IDbConnection _connection;
        private MetaWorkflowsProvider _provider;

        public WorkflowsService(ISystemObjectProviderFactory providerFactory,
            IWorkflowHost host,
            IDefinitionLoader definitionLoader,
            IWorkflowRegistry wRegistry,  
            IHttpContextAccessor httpContextAccessor, 
            ILoggerConfigService loggerConfigService)
        {
            _providerFactory = providerFactory;
            _host = host;
            _wRegistry = wRegistry;
            _httpContextAccessor = httpContextAccessor;
            _loggerConfigService = loggerConfigService;

        }

        public void SetUp(IDbConnection connection)
        {
            _connection = connection;

            _providerFactory.SetUp(_connection);
            _provider = _providerFactory.Create<MetaWorkflowsProvider>();
        }

        public async Task<ResultWrapper<WorkflowStartResultDto>> StartAsync(WorkflowStartDto startDto)
        {
            var result = new ResultWrapper<WorkflowStartResultDto>();
            var name = startDto.Name;

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
                    workflowDefinition = WorkflowBuilder.Build(workflowSettings);

                    // Register the workflow definition dynamically
                    _wRegistry.RegisterWorkflow(workflowDefinition);
                }
                else
                {
                    workflowDefinition = _wRegistry.GetDefinition(workflowSettings.Name);
                }

                var startResultDto = new WorkflowStartResultDto();
                foreach(var step in workflowDefinition.Steps)
                {
                    var stepDto = new WorkflowStepDto();
                    stepDto.Id = step.Id;
                    stepDto.Name = step.Name;
                    if (step.Id == 0)
                    {
                        stepDto.Title = DictMain.Start;
                    }

                    if (!string.IsNullOrEmpty(step.ExternalId))
                    {
                        if (Guid.TryParse(step.ExternalId, out var uid))
                        {
                            var stepSettings = workflowSettings.Steps.FirstOrDefault(x => x.Uid == uid);
                            stepDto.Title = stepSettings?.Title ?? string.Empty;
                        }
                    }

                    startResultDto.Steps.Add(stepDto);
                   
                }

                // Start the workflow
                var loggerConfig = await _loggerConfigService.GetLoggerConfigAsync(_connection, null);

                var workflowData = new BaSysWorkflowData();
                workflowData.Parameters = WorkflowParametersParser.Parse(startDto.Parameters);
                workflowData.LoggerConfig = loggerConfig;

                var loggerContext = new WorkflowLoggerContext();
                loggerContext.WorkflowUid = workflowSettings.Uid;
                loggerContext.WorkflowName = workflowSettings.Name;
                loggerContext.WorkflowTitle = workflowSettings.Title;
                loggerContext.Version = workflowSettings.Version;

                var user = _httpContextAccessor.HttpContext?.User;
                if (user != null)
                {
                    var userUid = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                    var userName = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                    var dbName = user.Claims.FirstOrDefault(x => x.Type == "DbName")?.Value;

                    loggerContext.UserUid = userUid;
                    loggerContext.UserName = userName;
                    loggerContext.Origin = "interactive";

                }

                workflowData.LoggerContext = loggerContext;

                string runUid = await _host.StartWorkflow(workflowSettings.Name, workflowData, Guid.NewGuid().ToString());
                loggerContext.RunUid = runUid;
                startResultDto.RunUid = runUid;

                result.Success(startResultDto, $"Workflow \"{name}\" started");
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

                if (instanse.Data is BaSysWorkflowData workflowData)
                {
                    dto.Messages = workflowData.Log;
                }

                foreach(var executionPointer in instanse.ExecutionPointers)
                {
                    var newStep = new WorkflowStepDto();
                    newStep.Status = (int)executionPointer.Status;
                    newStep.Name = executionPointer.StepName;
                    newStep.Id = executionPointer.StepId;

                    dto.Steps.Add(newStep);
                }

                result.Success(dto);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get Workflow status by reference {runUid}: {ex.Message}", ex.StackTrace);
            }

            return result;

        }

      
    }
}

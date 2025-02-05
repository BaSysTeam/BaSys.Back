using BaSys.Admin.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Workflows.DTO;
using BaSys.Workflows.Infrastructure;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BaSys.Admin.Services
{
    public class WorkflowsBoardService: IWorkflowsBoardService
    {
        private readonly IPersistenceProvider _persistenceProvider;

        public WorkflowsBoardService(IWorkflowHost host, IPersistenceProvider provider)
        {
            _persistenceProvider = provider;
        }

        public async Task<ResultWrapper<IEnumerable<WorkflowInfoDto>>> GetInfoAsync()
        {
            var result = new ResultWrapper<IEnumerable<WorkflowInfoDto>>();

            try
            {
                var identifiers = await _persistenceProvider.GetRunnableInstances(DateTime.MinValue);
                var activeWorkflows = await _persistenceProvider.GetWorkflowInstances(identifiers);

                var info = new List<WorkflowInfoDto>();

                foreach (var workflow in activeWorkflows)
                {
                    if (workflow == null) continue;

                    var infoItem = PrepareInfo(workflow);
                    info.Add(infoItem);
                }
                info = info.OrderBy(x => x.CreateTime).ToList();

                result.Success(info);
            }
            catch(Exception ex)
            {
                result.Error(-1, $"Cannot get workflows status: {ex.Message}.", ex.StackTrace);
            }

            return result;
        }

        private WorkflowInfoDto PrepareInfo(WorkflowInstance? workflow)
        {
            var infoItem = new WorkflowInfoDto();

            if (workflow == null) return infoItem;

            infoItem.Status = (int)workflow.Status;
            infoItem.CreateTime = workflow.CreateTime;
            infoItem.WorkflowName = workflow.WorkflowDefinitionId;

            if (workflow.Data is BaSysWorkflowData workflowData)
            {
                infoItem.WorkflowTitle = workflowData.LoggerContext.WorkflowTitle;
                infoItem.WorkflowUid = workflowData.LoggerContext.WorkflowUid.ToString();
                infoItem.Origin = workflowData.LoggerContext.Origin;
                infoItem.UserName = workflowData.LoggerContext.UserName;
                infoItem.RunUid = workflowData.LoggerContext.RunUid;
                infoItem.UserUid = workflowData.LoggerContext.UserUid;
            }

            return infoItem;
        }
    }
}

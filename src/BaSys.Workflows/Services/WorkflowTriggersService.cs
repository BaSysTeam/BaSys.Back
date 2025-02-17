using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Metadata.Models.WorkflowModel.TriggerEvents;
using BaSys.Translation;
using BaSys.Workflows.Abstractions;
using BaSys.Workflows.DTO;
using System.Data;
using System.Net.WebSockets;

namespace BaSys.Workflows.Services
{
    public sealed class WorkflowTriggersService: IWorkflowTriggersService
    {
        private IDbConnection? _connection;
        private WorkflowTriggersProvider? _provider;
        private readonly ISystemObjectProviderFactory _providerFactory;

        public WorkflowTriggersService(ISystemObjectProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public void SetUp(IDbConnection connection)
        {
            _connection = connection;
            _providerFactory.SetUp(_connection);

            _provider = _providerFactory.Create<WorkflowTriggersProvider>();
        }

        public async Task<ResultWrapper<IEnumerable<WorkflowTriggerDto>>> GetCollectionAsync(Guid? metaObjectUid, Guid? workflowUid)
        {
            var result = new ResultWrapper<IEnumerable<WorkflowTriggerDto>>();

            try
            {
                var workflowsProvider = _providerFactory.Create<MetaWorkflowsProvider>();
                var allWorkflows = (await workflowsProvider.GetCollectionAsync(null)).ToDictionary(x=>x.Uid, x=>x);
                var allEvents = WorkflowTriggerEvents.AllItems().ToDictionary(x=>x.Uid, x=>x);

                var collection = await _provider.GetCollectionAsync(metaObjectUid, workflowUid, null);
                var collectionDto = new List<WorkflowTriggerDto>();
                foreach(var item in collection)
                {
                    var dto = new WorkflowTriggerDto
                    {
                        Uid = item.Uid,
                        MetaObjectKindUid = item.MetaObjectKindUid,
                        MetaObjectUid = item.MetaObjectUid,
                        WorkflowUid = item.WorkflowUid,
                        EventUid = item.EventUid,
                        Memo = item.Memo,
                        IsActive = item.IsActive

                    };

                    dto.EventName = allEvents[dto.EventUid].Name;
                    if (allWorkflows.ContainsKey(dto.WorkflowUid))
                    {
                        dto.WorkflowTitle = allWorkflows[dto.WorkflowUid].Title;
                    }

                    collectionDto.Add(dto);
                }
                result.Success(collectionDto);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotGetData}: {ex.Message}", ex.StackTrace);
            }

            return result;
        }
        public async Task<ResultWrapper<WorkflowTrigger>> GetItemAsync(Guid uid)
        {
            var result = new ResultWrapper<WorkflowTrigger>();

            try
            {
                var item = await _provider.GetItemAsync(uid, null);
                if (item == null)
                {
                    result.Error(-1, $"{DictMain.CannotFindItem}: {uid}");
                    return result;
                }

                result.Success(item);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotGetData}: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<Guid>> CreateAsync(WorkflowTrigger item)
        {
            var result = new ResultWrapper<Guid>();

            try
            {
                var newUid = await _provider.InsertAsync(item, null);
                result.Success(newUid, DictMain.ItemSaved);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotCreateItem}: {ex.Message}", ex.StackTrace);

            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateAsync(WorkflowTrigger record)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var savedRecord = await _provider.GetItemAsync(record.Uid, null);
                if (savedRecord == null)
                {
                    result.Error(-1, $"{DictMain.CannotFindItem} WorkflowScheduleRecord.{record.Uid}");
                    return result;
                }

                savedRecord.CopyFrom(record);
                var insertedCount = await _provider.UpdateAsync(savedRecord, null);
                result.Success(insertedCount, DictMain.ItemSaved);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotUpdateItem}: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteAsync(Guid uid)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var deletedCount = await _provider.DeleteAsync(uid, null);
                result.Success(deletedCount, DictMain.ItemDeleted);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotDeleteItem} WorkflowTrigger.{uid}: {ex.Message}", ex.StackTrace);
            }


            return result;
        }


    }
}

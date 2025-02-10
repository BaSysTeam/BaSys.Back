using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Translation;
using BaSys.Workflows.Abstractions;
using System.Data;

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

        public async Task<ResultWrapper<IEnumerable<WorkflowTrigger>>> GetCollectionAsync(Guid? metaObjectUid, Guid? workflowUid)
        {
            var result = new ResultWrapper<IEnumerable<WorkflowTrigger>>();

            try
            {
                var collection = await _provider.GetCollectionAsync(metaObjectUid, workflowUid, null);
                result.Success(collection);
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

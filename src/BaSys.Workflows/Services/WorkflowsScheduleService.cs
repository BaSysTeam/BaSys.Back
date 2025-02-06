using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Translation;
using BaSys.Workflows.Abstractions;
using System.Data;

namespace BaSys.Workflows.Services
{
    public sealed class WorkflowsScheduleService : IWorkflowsScheduleService
    {
        private IDbConnection? _connection;
        private WorkflowScheduleProvider _provider;

        public void SetUp(IDbConnection connection)
        {
            _connection = connection;
            _provider = new WorkflowScheduleProvider(_connection);
        }

        public async Task<ResultWrapper<IEnumerable<WorkflowScheduleRecord>>> GetCollectionAsync(Guid? workflowUid, bool? isActive)
        {
            var result = new ResultWrapper<IEnumerable<WorkflowScheduleRecord>>();

            try
            {
                var collection = await _provider.GetCollectionAsync(workflowUid, isActive, null);
                result.Success(collection);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get data: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<WorkflowScheduleRecord>> GetRecordAsync(Guid uid)
        {
            var result = new ResultWrapper<WorkflowScheduleRecord>();

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
                result.Error(-1, $"Cannot get data: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<Guid>> CreateAsync(WorkflowScheduleRecord record)
        {
            var result = new ResultWrapper<Guid>();

            try
            {
                var newUid = await _provider.InsertAsync(record, null);
                result.Success(newUid, DictMain.ItemSaved);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotCreateItem}: {ex.Message}", ex.StackTrace);

            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateAsync(WorkflowScheduleRecord record)
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
                result.Error(-1, $"{DictMain.CannotDeleteItem} WorkflowSheduleRecord.{uid}: {ex.Message}", ex.StackTrace);
            }


            return result;
        }
    }
}

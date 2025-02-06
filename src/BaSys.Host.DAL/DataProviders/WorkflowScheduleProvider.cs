using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Models.WorkflowModel;
using Dapper;
using System.Data;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class WorkflowScheduleProvider : SystemObjectProviderBase<WorkflowScheduleRecord>
    {
        public WorkflowScheduleProvider(IDbConnection connection) : base(connection, new WorkflowScheduleRecordConfiguration())
        {

        }

        public async Task<IEnumerable<WorkflowScheduleRecord>> GetCollectionAsync(Guid? workflowUid, bool? isActive, IDbTransaction? transaction)
        {
            var builder = SelectBuilder.Make()
                .From(_config.TableName)
                .Select("*");

            if (isActive.HasValue)
            {
                builder.WhereAnd("isactive = @isActive")
                    .Parameter("isActive", isActive.Value, DbType.Boolean);
            }

            if (workflowUid.HasValue)
            {
                builder.WhereAnd("workflowuid = @workflowUid")
                    .Parameter("workflowUid", workflowUid.Value, DbType.Guid);
            }

            _query = builder.Query(_sqlDialect);

            var result = await _dbConnection.QueryAsync<WorkflowScheduleRecord>(_query.Text, _query.DynamicParameters, transaction);

            return result;
        }

        public override async Task<Guid> InsertAsync(WorkflowScheduleRecord item, IDbTransaction? transaction)
        {
            _query = InsertBuilder.Make(_config).FillValuesByColumnNames(true).Query(_sqlDialect);

            item.BeforeSave();
            var insertedCount = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return InsertedUid(insertedCount, item.Uid);
        }

        public override async Task<int> UpdateAsync(WorkflowScheduleRecord item, IDbTransaction? transaction)
        {
            var result = 0;

            _query = UpdateBuilder.Make(_config)
              .WhereAnd("uid = @uid")
              .Query(_sqlDialect);

            item.BeforeSave();
            result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return result;
        }
    }
}

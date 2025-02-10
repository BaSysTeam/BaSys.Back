using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Models.WorkflowModel;
using Dapper;
using System.Data;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class WorkflowTriggersProvider: SystemObjectProviderBase<WorkflowTrigger>
    {
        public WorkflowTriggersProvider(IDbConnection connection):base(connection, new WorkflowTriggerConfiguration())
        {
            
        }

        public override async Task<Guid> InsertAsync(WorkflowTrigger item, IDbTransaction? transaction)
        {
            _query = InsertBuilder.Make(_config).FillValuesByColumnNames(true).Query(_sqlDialect);

            item.BeforeSave();
            var insertedCount = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

            return InsertedUid(insertedCount, item.Uid);
        }

        public override async Task<int> UpdateAsync(WorkflowTrigger item, IDbTransaction? transaction)
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

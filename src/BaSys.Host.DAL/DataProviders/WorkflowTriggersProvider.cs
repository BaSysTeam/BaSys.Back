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

        public async Task<IEnumerable<WorkflowTrigger>> GetCollectionAsync(Guid? metaObjectUid, Guid? workflowUid, IDbTransaction? transaction)
        {
            var builder = SelectBuilder
                .Make()
                .From(_config.TableName)
                .Select("*");

            if (metaObjectUid.HasValue)
            {
                builder.WhereAnd("metaobjectuid = @metaObjectUid")
                    .Parameter("metaObjectUid", metaObjectUid, DbType.Guid);
            }

            if (workflowUid.HasValue)
            {
                builder.WhereAnd("workflowuid = @workflowUid")
                    .Parameter("workflowUid", workflowUid, DbType.Guid);
            }

            _query = builder.Query(_sqlDialect);

            var result = await _dbConnection.QueryAsync<WorkflowTrigger>(_query.Text, _query.DynamicParameters, transaction);

            return result;
        }

    }
}

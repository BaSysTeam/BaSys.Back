using BaSys.Common.Enums;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Helpers;
using BaSys.Logging.Workflow;
using Dapper;
using System.Data;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class WorkflowLogRecordsProvider
    {
        private readonly IDbConnection _connection;
        private readonly SqlDialectKinds _sqlDialect;
        private readonly string _tableName;

        private IQuery? _query;

        public IQuery? LastQuery => _query;

        public WorkflowLogRecordsProvider(IDbConnection connection, string tableName)
        {
            _connection = connection;
            _sqlDialect = SqlDialectKindHelper.GetDialectKind(_connection);

            _tableName = tableName;
        }

        public async  Task<IEnumerable<WorkflowLogRecord>> GetLifecycleRecordsAsync(Guid workflowGuid, IDbTransaction? transaction)
        {
            _query = SelectBuilder.Make()
                .From(_tableName)
                .Select("*")
                .WhereAnd("workflow_uid = @workflowUid")
                .WhereAnd("(kind = @kindStart or kind = @kindStop)")
                .Parameter("workflowUid", workflowGuid, DbType.Guid)
                .Parameter("kindStart", WorkflowLogEventKinds.Start)
                .Parameter("kindStop", WorkflowLogEventKinds.Stop)
                .OrderBy("raise_date desc")
                .Query(_sqlDialect);

            var dynamicResult = await _connection.QueryAsync<dynamic>(_query.Text, _query.DynamicParameters, transaction);
            var result = MapDynamic(dynamicResult);
           

            return result;
        }

        public async Task<IEnumerable<WorkflowLogRecord>> GetAllRecordsByRunAsync(string runUid, IDbTransaction? transaction)
        {
            _query = SelectBuilder.Make()
                .From(_tableName)
                .Select("*")
                .WhereAnd("run_uid = @runUid")
                .Parameter("runUid", runUid, DbType.String)
                .OrderBy("raise_date")
                .Query(_sqlDialect);

            var dynamicResult = await _connection.QueryAsync<dynamic>(_query.Text, _query.DynamicParameters, transaction);
            var result = MapDynamic(dynamicResult);


            return result;
        }

        public async Task<int> DeleteRecordsAsync(Guid workflowGuid, IDbTransaction? transaction)
        {
            _query = DeleteBuilder.Make()
                .Table(_tableName)
                .WhereAnd("workflow_uid = @workflowUid")
                .Parameter("workflowUid", workflowGuid, DbType.Guid)
                .Query(_sqlDialect);

            var deletedCount = await _connection.ExecuteAsync(_query.Text, _query.DynamicParameters, transaction);

            return deletedCount;
        }

        private IList<WorkflowLogRecord> MapDynamic(IEnumerable<dynamic> source)
        {
            var result = source.Select(x => new WorkflowLogRecord()
            {
                Kind = (WorkflowLogEventKinds)x.kind,
                Level = (EventTypeLevels)x.level,
                LogMessage = x.log_message,
                Origin = x.origin,
                RaiseDate = x.raise_date,
                RunUid = x.run_uid,
                StepName = x.step_name,
                UserName = x.user_name,
                UserUid = x.user_uid,
                WorkflowUid = x.workflow_uid
            }).ToList();

            return result;
        }
    }
}

using BaSys.Common.Abstractions;
using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.Workflow;
using System.Data;

namespace BaSys.Constructor.Services
{
    public class WorkflowLogsService : IWorkflowLogsService
    {
        private readonly ILoggerConfigService _loggerConfigService;
        private readonly IMainConnectionFactory _connectionFactory;

        public WorkflowLogsService(IMainConnectionFactory connectionFactory, 
            ILoggerConfigService loggerConfigService)
        {
            _loggerConfigService = loggerConfigService;
            _connectionFactory = connectionFactory;
        }

        public async Task<ResultWrapper<IEnumerable<WorkflowLogRecord>>> GetLifecycleRecordsAsync(string workflowUidStr)
        {
            var result = new ResultWrapper<IEnumerable<WorkflowLogRecord>>();

            try
            {
                result = await ExecuteGetLifecycleRecordsAsync(workflowUidStr);
            }
            catch(Exception ex)
            {
                result.Error(-1, $"Cannot get workflow log: {ex.Message}.", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<IEnumerable<WorkflowLogRecord>>> GetRecordsByRunAsync(string runUid)
        {
            var result = new ResultWrapper<IEnumerable<WorkflowLogRecord>>();

            try
            {
                result = await ExecuteGetRecordsByRunAsync(runUid);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get workflow log: {ex.Message}.", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteWorkflowRecordsAsync(string workflowUidStr)
        {
            var result = new ResultWrapper<int>();

            try
            {
                result = await ExecuteDeleteWorkflowRecordsAsync(workflowUidStr);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot delete workflow records: {ex.Message}.", ex.StackTrace);
            }

            return result;
        }

        private async Task<ResultWrapper<IEnumerable<WorkflowLogRecord>>> ExecuteGetLifecycleRecordsAsync(string workflowUidStr)
        {
            var result = new ResultWrapper<IEnumerable<WorkflowLogRecord>>();

            if (!Guid.TryParse(workflowUidStr, out var workflowUid))
            {
                result.Error(-1, $"Cannot parse Guid: {workflowUidStr}");
                return result;
            }

            var loggerConfig = await GetLoggerConfigAsync(result);
            if (loggerConfig == null) return result;

            if (loggerConfig.LoggerType == LoggerTypes.MongoDb)
            {
                result.Error(-1, $"Logger is not implemented for this DB kind {loggerConfig.LoggerType}");
            }
            else
            {
                var dbKind = DbKinds.PgSql;
                if (loggerConfig.LoggerType == LoggerTypes.MsSql)
                {
                    dbKind = DbKinds.MsSql;
                }

                using (IDbConnection conection = _connectionFactory.CreateConnection(loggerConfig.ConnectionString, dbKind))
                {
                    var provider = new WorkflowLogRecordsProvider(conection, loggerConfig.WorkflowsLogTableName);
                    var records = await provider.GetLifecycleRecordsAsync(workflowUid, null);
                    result.Success(records);
                }
            }

            return result;
        }

        private async Task<ResultWrapper<IEnumerable<WorkflowLogRecord>>> ExecuteGetRecordsByRunAsync(string runUid)
        {
            var result = new ResultWrapper<IEnumerable<WorkflowLogRecord>>();

            var loggerConfig = await GetLoggerConfigAsync(result);
            if (loggerConfig == null) return result;

            if (loggerConfig.LoggerType == LoggerTypes.MongoDb)
            {
                result.Error(-1, $"Logger is not implemented for this DB kind {loggerConfig.LoggerType}");
            }
            else
            {
                var dbKind = DbKinds.PgSql;
                if (loggerConfig.LoggerType == LoggerTypes.MsSql)
                {
                    dbKind = DbKinds.MsSql;
                }

                using (IDbConnection conection = _connectionFactory.CreateConnection(loggerConfig.ConnectionString, dbKind))
                {
                    var provider = new WorkflowLogRecordsProvider(conection, loggerConfig.WorkflowsLogTableName);
                    var records = await provider.GetAllRecordsByRunAsync(runUid, null);
                    result.Success(records);
                }
            }

            return result;
        }
        private async Task<ResultWrapper<int>> ExecuteDeleteWorkflowRecordsAsync(string workflowUidStr)
        {
            var result = new ResultWrapper<int>();

            if (!Guid.TryParse(workflowUidStr, out var workflowUid))
            {
                result.Error(-1, $"Cannot parse Guid: {workflowUidStr}");
                return result;
            }

            var loggerConfig = await GetLoggerConfigAsync(result);
            if (loggerConfig == null) return result;

            if (loggerConfig.LoggerType == LoggerTypes.MongoDb)
            {
                result.Error(-1, $"Logger is not implemented for this DB kind {loggerConfig.LoggerType}");
            }
            else
            {
                var dbKind = DbKinds.PgSql;
                if (loggerConfig.LoggerType == LoggerTypes.MsSql)
                {
                    dbKind = DbKinds.MsSql;
                }

                using (IDbConnection conection = _connectionFactory.CreateConnection(loggerConfig.ConnectionString, dbKind))
                {
                    var provider = new WorkflowLogRecordsProvider(conection, loggerConfig.WorkflowsLogTableName);
                    var deletedCount = await provider.DeleteRecordsAsync(workflowUid, null);
                    result.Success(deletedCount, $"Deleted records: {deletedCount}");
                }
            }

            return result;
        }

        private async Task<BaSys.Logging.Abstractions.LoggerConfig?> GetLoggerConfigAsync(IResultWrapper result)
        {

            var loggerConfig = await _loggerConfigService.GetLoggerConfig();

            if (loggerConfig == null)
            {
                result.Error(-1, "Cannot get logger config.");
                return loggerConfig;
            }

            if (!loggerConfig.IsEnabled)
            {
                result.Error(-1, "Logger is disabled. Enable logger in app settings.");
                return loggerConfig;
            }

            if (loggerConfig.LoggerType == LoggerTypes.MsSql
                || loggerConfig.LoggerType == LoggerTypes.MongoDb)
            {
                result.Error(-1, $"Logger is not implemented for this DB kind {loggerConfig.LoggerType}");
            }

            return loggerConfig;
        }


    }
}

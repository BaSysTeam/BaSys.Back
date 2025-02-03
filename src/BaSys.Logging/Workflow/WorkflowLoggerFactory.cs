using BaSys.Common.Enums;
using BaSys.Logging.Abstractions;
using BaSys.Logging.Workflow.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Logging.Workflow
{
    public sealed class WorkflowLoggerFactory : IWorkflowLoggerFactory
    {
        private readonly LoggerConfig _loggerConfig;
        private readonly WorkflowLoggerContext _loggerContext;

        public WorkflowLoggerFactory(LoggerConfig config, WorkflowLoggerContext context)
        {
            _loggerConfig = config;
            _loggerContext = context;
        }

        public IWorkflowLogger? GetLogger()
        {
            switch (_loggerConfig.LoggerType)
            {
                case LoggerTypes.PgSql:
                    return new WorkflowPgSqlLogger(_loggerConfig, _loggerContext);
                case LoggerTypes.MsSql:
                    // TODO: Implement MSSQL logger.
                    return null;
                case LoggerTypes.MongoDb:
                    // TODO: Implement MongoDb logger.
                    return null;
                default:
                    return null;

            }
        }

    }
}

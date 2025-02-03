using BaSys.Common.Enums;
using BaSys.Logging.Abstractions;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Logging.Workflow.Abstractions
{
    public abstract class WorkflowLogger : IWorkflowLogger
    {
        protected readonly LoggerConfig _loggerConfig;

        protected readonly string _origine;
        protected readonly string _workflowName;
        protected readonly Guid _workflowUid;
        protected readonly string _runUid;

        protected readonly string _userName;
        protected readonly string _userUid;

        protected Logger? Logger;

        public WorkflowLogger(LoggerConfig loggerConfig, WorkflowLoggerContext context)
        {
            _loggerConfig = loggerConfig;

            _origine = context.Origine;

            _workflowName = context.WorkflowName;
            _workflowUid = context.WorkflowUid;
            _runUid = context.RunUid;

            _userName = context.UserName;
            _userUid = context.UserUid;
        }

        public void Write(WorkflowLogEventKinds kind, EventTypeLevels level, string stepName, string message)
        {
            if (!CanWrite(level))
            {
                return;
            }

            WriteInner(kind, level, stepName, message);
        }

        public void Info(WorkflowLogEventKinds kind, string stepName, string message)
        {
            var level = EventTypeLevels.Info;
            if (!CanWrite(level))
            {
                return;
            }

            WriteInner(kind, level, stepName, message);
        }

        public void Error(WorkflowLogEventKinds kind, string stepName, string message)
        {
            var level = EventTypeLevels.Error;
            if (!CanWrite(level))
            {
                return;
            }

            WriteInner(kind, level, stepName, message);
        }

        protected abstract void WriteInner(WorkflowLogEventKinds kind, EventTypeLevels level, string stepName, string message);

        private bool CanWrite(EventTypeLevels level)
        {
            if (!_loggerConfig.IsEnabled ||
                level < _loggerConfig.MinimumLogLevel)
                return false;
            return true;
        }


    }
}

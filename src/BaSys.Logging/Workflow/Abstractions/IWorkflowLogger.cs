using BaSys.Common.Enums;

namespace BaSys.Logging.Workflow.Abstractions
{
    public interface IWorkflowLogger
    {
        void Error(WorkflowLogEventKinds kind, string stepName, string message);
        void Info(WorkflowLogEventKinds kind, string stepName, string message);
        void Write(WorkflowLogEventKinds kind, EventTypeLevels level, string stepName, string message);
    }
}
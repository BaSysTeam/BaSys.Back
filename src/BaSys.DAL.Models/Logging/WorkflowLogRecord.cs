using BaSys.Common.Enums;

namespace BaSys.Logging.Workflow
{
    public sealed class WorkflowLogRecord
    {
        public DateTime RaiseDate { get; set; }
        public string LogMessage { get; set; } = string.Empty;
        public WorkflowLogEventKinds Kind { get; set; }
        public EventTypeLevels Level { get; set; }
        public string Origin { get; set; } = string.Empty;
        public Guid WorkflowUid { get; set; }
        public string RunUid { get; set; } = string.Empty;
        public string StepName { get; set; } = string.Empty;
        public string UserUid { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;



    }
}

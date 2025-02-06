namespace BaSys.Logging.Workflow
{
    public sealed class WorkflowLoggerContext
    {
        public string Origin { get; set; } = string.Empty;
        public string WorkflowTitle { get; set; } = string.Empty;
        public string WorkflowName { get; set; } = string.Empty;
        public Guid WorkflowUid { get; set; }
        public string RunUid { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserUid { get; set; } = string.Empty;
        public long Version { get; set; }

        public WorkflowLoggerContext()
        {
            
        }
       
    }

}

namespace BaSys.Logging.Workflow
{
    public sealed class WorkflowLoggerContext
    {
        public string Origin { get; set; } = string.Empty;
        public string WorkflowName { get; set; } = string.Empty;
        public Guid WorkflowUid { get; set; }
        public string RunUid { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserUid { get; set; } = string.Empty;

        public WorkflowLoggerContext()
        {
            
        }

        public WorkflowLoggerContext(string origin,
                                     Guid dbUid,
                                     string workflowName,
                                     Guid workflowUid,
                                     string runUid,
                                     string userName,
                                     string userUid)
        {
            Origin = origin;
            WorkflowName = workflowName;
            WorkflowUid = workflowUid;
            RunUid = runUid;
            UserName = userName;
            UserUid = userUid;
        }
    }

}

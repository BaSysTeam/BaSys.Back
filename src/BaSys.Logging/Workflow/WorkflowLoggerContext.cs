namespace BaSys.Logging.Workflow
{
    public readonly struct WorkflowLoggerContext
    {
        public Guid DbUid { get; }
        public string WorkflowName { get; } = string.Empty;
        public Guid WorkflowUid { get; }
        public string RunUid { get; } = string.Empty;
        public string UserName { get; } = string.Empty;
        public string UserUid { get; } = string.Empty;

        public WorkflowLoggerContext()
        {
            
        }

        public WorkflowLoggerContext(Guid dbUid, string workflowName, Guid workflowUid, string runUid, string userName, string userUid)
        {
            DbUid = dbUid;
            WorkflowName = workflowName;
            WorkflowUid = workflowUid;
            RunUid = runUid;
            UserName = userName;
            UserUid = userUid;
        }
    }

}

namespace BaSys.Logging.Workflow
{
    public sealed class WorkflowLoggerContext
    {
        public string DbName { get; set; } = string.Empty;
        public Guid DbUid { get; set; }
        public string WorkflowName { get; set; } = string.Empty;
        public Guid WorkflowUid { get; set; }
        public string RunUid { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserUid { get; set; } = string.Empty;

        public WorkflowLoggerContext()
        {
            
        }

        public WorkflowLoggerContext(string dbName,
                                     Guid dbUid,
                                     string workflowName,
                                     Guid workflowUid,
                                     string runUid,
                                     string userName,
                                     string userUid)
        {
            DbUid = dbUid;
            DbName = dbName;
            WorkflowName = workflowName;
            WorkflowUid = workflowUid;
            RunUid = runUid;
            UserName = userName;
            UserUid = userUid;
        }
    }

}

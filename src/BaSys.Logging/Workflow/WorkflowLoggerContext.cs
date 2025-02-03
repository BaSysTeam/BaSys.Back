namespace BaSys.Logging.Workflow
{
    public sealed class WorkflowLoggerContext
    {
        public string Origine { get; set; } = string.Empty;
        public string WorkflowName { get; set; } = string.Empty;
        public Guid WorkflowUid { get; set; }
        public string RunUid { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserUid { get; set; } = string.Empty;

        public WorkflowLoggerContext()
        {
            
        }

        public WorkflowLoggerContext(string origine,
                                     Guid dbUid,
                                     string workflowName,
                                     Guid workflowUid,
                                     string runUid,
                                     string userName,
                                     string userUid)
        {
            Origine = origine;
            WorkflowName = workflowName;
            WorkflowUid = workflowUid;
            RunUid = runUid;
            UserName = userName;
            UserUid = userUid;
        }
    }

}

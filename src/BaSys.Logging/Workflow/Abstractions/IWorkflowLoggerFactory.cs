namespace BaSys.Logging.Workflow.Abstractions
{
    public interface IWorkflowLoggerFactory
    {
        IWorkflowLogger? GetLogger();
    }
}

using BaSys.Logging.Abstractions;
using BaSys.Logging.InMemory;
using BaSys.Logging.Workflow;

namespace BaSys.Workflows.Infrastructure
{
    public sealed class BaSysWorkflowData
    {
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public LoggerConfig LoggerConfig { get; set; } = new LoggerConfig();
        public WorkflowLoggerContext LoggerContext { get; set; } = new WorkflowLoggerContext();
        public List<InMemoryLogMessage> Log { get; set; } = new List<InMemoryLogMessage>();

    }
}

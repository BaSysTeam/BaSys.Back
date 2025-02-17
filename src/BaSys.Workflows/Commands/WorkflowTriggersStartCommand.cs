using BaSys.Logging.Abstractions;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.SuperAdmin.DAL.Models;

namespace BaSys.Workflows.Commands
{
    public sealed class WorkflowTriggersStartCommand
    {
        public string? UserUid { get; set; }
        public string? UserName { get; set; }
        public DbInfoRecord? DbInfoRecord {get; set; }
        public LoggerConfig? LoggerConfig { get; set; }
        public List<WorkflowTrigger> Triggers { get; set; } = new List<WorkflowTrigger>();
        public Dictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();
    
    }
}

using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models.WorkflowModel
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class WorkflowSettings
    {
        public Guid Uid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public long Version { get; set; }

        public List<IWorkflowStepSettings> Steps { get; set; } = new List<IWorkflowStepSettings>();
    }
}

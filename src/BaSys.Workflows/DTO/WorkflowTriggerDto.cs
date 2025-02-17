using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Workflows.DTO
{
    public sealed class WorkflowTriggerDto
    {
        public Guid Uid { get; set; }
        public Guid MetaObjectKindUid { get; set; }
        public Guid MetaObjectUid { get; set; }
        public Guid EventUid { get; set; }
        public string EventName { get; set; } = string.Empty;
        public Guid WorkflowUid { get; set; }
        public string WorkflowTitle { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

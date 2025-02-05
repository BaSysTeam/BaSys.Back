using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Workflows.DTO
{
    public sealed class WorkflowInfoDto
    {
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public string RunUid { get; set; } = string.Empty;
        public string WorkflowUid { get; set; } = string.Empty;
        public string WorkflowName { get; set; } = string.Empty;
        public string WorkflowTitle { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserUid { get; set; } = string.Empty;
    }
}

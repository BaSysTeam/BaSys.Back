using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Workflows.DTO
{
    public sealed class WorkflowStartDto
    {
        public string RunUid { get; set; } = string.Empty;

        public List<WorkflowStepDto> Steps { get; set; } = new List<WorkflowStepDto>();
    }
}

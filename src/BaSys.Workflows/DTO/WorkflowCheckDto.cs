using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowCore.Models;

namespace BaSys.Workflows.DTO
{
    public sealed class WorkflowCheckDto
    {
        public string Id { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public WorkflowStatus Status { get; set; }
    }
}

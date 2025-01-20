using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models.WorkflowModel.Abstractions
{

    public interface IWorkflowStepSettings
    {
        Guid Uid { get; set; }
        IWorkflowStepKind Kind { get; }
        string Title { get; set; }
        string Name { get; set; }
        string Memo { get; set; }
        bool IsActive { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models.WorkflowModel.Abstractions
{
    public interface IWorkflowStepKind
    {
        Guid Uid { get; }
        string Name { get; }
    }
}

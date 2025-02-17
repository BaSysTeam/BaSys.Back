using System;

namespace BaSys.Metadata.Models.WorkflowModel.Abstractions
{
    public interface IWorkflowStepKind
    {
        Guid Uid { get; }
        string Name { get; }
    }
}

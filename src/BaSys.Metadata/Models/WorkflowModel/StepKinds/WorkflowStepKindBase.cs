using BaSys.Metadata.Models.WorkflowModel.Abstractions;
using System;

namespace BaSys.Metadata.Models.WorkflowModel.StepKinds
{
    public abstract class WorkflowStepKindBase : IWorkflowStepKind
    {
        public abstract Guid Uid { get; }
        public abstract string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}

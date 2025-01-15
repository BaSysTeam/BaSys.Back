using System;

namespace BaSys.Metadata.Models.WorkflowModel
{
    public abstract class WorkflowStepKindBase: IWorkflowStepKind
    {
        public abstract Guid Uid { get; }
        public abstract string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}

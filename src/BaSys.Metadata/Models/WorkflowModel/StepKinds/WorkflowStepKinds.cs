using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Metadata.Models.WorkflowModel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaSys.Metadata.Models.WorkflowModel.StepKinds
{
    public static class WorkflowStepKinds
    {
        public static readonly IWorkflowStepKind Sleep = new SleepStepKind();
        public static readonly IWorkflowStepKind Message = new MessageStepKind();

        public static IList<IWorkflowStepKind> AllKinds()
        {
            var collection = new List<IWorkflowStepKind> {
                Sleep,
                Message
            };

            return collection;
        }

        public static IWorkflowStepKind GetKind(Guid uid)
        {
            return AllKinds().Single(x => x.Uid == uid);
        }

        public static IWorkflowStepKind GetKindByName(string name)
        {
            return AllKinds().Single(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}

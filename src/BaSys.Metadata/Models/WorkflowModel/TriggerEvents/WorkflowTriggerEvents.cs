using BaSys.Metadata.Models.WorkflowModel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaSys.Metadata.Models.WorkflowModel.TriggerEvents
{
    public static class WorkflowTriggerEvents
    {
        public static readonly IWorkflowTriggerEvent Create = new CreateTriggerEvent();
        public static readonly IWorkflowTriggerEvent Update = new UpdateTriggerEvent();

        public static IEnumerable<IWorkflowTriggerEvent> AllItems()
        {
            var collection = new List<IWorkflowTriggerEvent>
            {
                Create,
                Update
            };

            return collection;
        }

        public static IWorkflowTriggerEvent GetItem(Guid uid)
        {
            return AllItems().Single(x => x.Uid == uid);
        }

        public static IWorkflowTriggerEvent GetByName(string name)
        {
            return AllItems().Single(x => x.Name.Equals(name, StringComparison.Ordinal));
        }
    }
}

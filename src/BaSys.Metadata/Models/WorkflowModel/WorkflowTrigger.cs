using BaSys.Common.Abstractions;
using BaSys.Metadata.Models.WorkflowModel.TriggerEvents;
using System;

namespace BaSys.Metadata.Models.WorkflowModel
{
    public sealed class WorkflowTrigger: SystemObjectBase
    {
        public Guid MetaObjectKindUid { get; set; }
        public Guid MetaObjectUid { get; set; }
        public Guid EventUid { get; set; }
        public Guid WorkflowUid { get; set; }
        public string Memo { get; set; }
        public bool IsActive { get; set; }

        public void CopyFrom(WorkflowTrigger source)
        {
            MetaObjectKindUid = source.MetaObjectKindUid;
            MetaObjectUid = source.MetaObjectUid;
            EventUid = source.EventUid;
            WorkflowUid = source.WorkflowUid;
            Memo = source.Memo;
            IsActive = source.IsActive;
        }

        public override string ToString()
        {
            var triggerEvent = WorkflowTriggerEvents.GetItem(EventUid);
            return $"{IsActive}. {MetaObjectUid}:{triggerEvent.Uid}->{WorkflowUid}. {Memo}";
        }
    }
}

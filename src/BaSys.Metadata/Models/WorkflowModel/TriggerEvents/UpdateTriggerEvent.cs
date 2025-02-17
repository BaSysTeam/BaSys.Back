using System;

namespace BaSys.Metadata.Models.WorkflowModel.TriggerEvents
{
    public sealed class UpdateTriggerEvent : TriggerEventBase
    {
        public override Guid Uid => Guid.Parse("{43D1B132-EC50-41DE-AB27-8395A5EC5F00}");
        public override string Name => "update";
    }
}

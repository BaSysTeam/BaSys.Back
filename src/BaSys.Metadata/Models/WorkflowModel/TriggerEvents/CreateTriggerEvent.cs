using System;

namespace BaSys.Metadata.Models.WorkflowModel.TriggerEvents
{
    public sealed class CreateTriggerEvent : TriggerEventBase
    {
        public override Guid Uid => Guid.Parse("{8C382B01-543B-42A9-849A-29578F3F2DDB}");
        public override string Name => "create";
    }
}

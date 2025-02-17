using BaSys.Logging.Abstractions;

namespace BaSys.Logging.EventTypes
{
    public sealed class TriggerStartEventType : EventType
    {
        public override Guid Uid => Guid.Parse("{AD9F512A-CF9B-4FFD-BDFC-264B8E79DEEB}");
        public override string EventName => "TriggerStart";
    }
}

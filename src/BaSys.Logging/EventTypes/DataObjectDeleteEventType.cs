using BaSys.Logging.Abstractions;

namespace BaSys.Logging.EventTypes
{
    public sealed class DataObjectDeleteEventType : EventType
    {
        public override Guid Uid => Guid.Parse("{1792D060-1F4D-45C7-A39F-D4485818724F}");
        public override string EventName => "DataObjectDelete";
    }
}

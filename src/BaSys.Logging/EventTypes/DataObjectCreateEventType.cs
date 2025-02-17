using BaSys.Logging.Abstractions;

namespace BaSys.Logging.EventTypes
{
    public sealed class DataObjectCreateEventType : EventType
    {
        public override Guid Uid => Guid.Parse("{B5B8167D-E4A3-4E10-893A-6FCCD7782823}");
        public override string EventName => "DataObjectCreate";
    }
}

using BaSys.Logging.Abstractions;

namespace BaSys.Logging.EventTypes
{
    public sealed class DataObjectUpdateEventType : EventType
    {
        public override Guid Uid => Guid.Parse("{CD79E6BD-A062-4606-B125-A3C5E2E2901B}");
        public override string EventName => "DataObjectUpdate";
    }
}

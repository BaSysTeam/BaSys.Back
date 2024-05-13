using BaSys.Logging.Abstractions;

namespace BaSys.Logging.EventTypes;

public class PublicApiEventType : EventType
{
    public override Guid Uid => new ("C8B24AAA-0EA1-4A17-B279-9A7B7EE65F59");
    public override string EventName => "PublicApi";
}
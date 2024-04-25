namespace BaSys.Logging.Abstractions;

public abstract class EventType
{
    public abstract Guid Uid { get; }
    public abstract string EventName { get; }
    public virtual string Module => "core";

    public override string ToString()
    {
        return EventName;
    }
}
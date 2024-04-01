namespace BaSys.Logging.Infrastructure;

public abstract class EventTypeBase
{
    public abstract Guid Uid { get; }
    public abstract string EventName { get; }
    public virtual string Module => "core";
}
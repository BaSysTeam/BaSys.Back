using BaSys.Logging.Abstractions;

namespace BaSys.Logging.EventTypes;

public class UserLoginFailEventType : EventType
{
    public override Guid Uid => new("fd918e91-6a2d-4c48-aa51-f61a7e216b31");
    public override string EventName => "UserLoginFail";
}
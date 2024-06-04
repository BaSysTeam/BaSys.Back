using BaSys.Logging.Abstractions;

namespace BaSys.Logging.EventTypes;
public class UserLoginEventType : EventType
{
    public override Guid Uid => new("54dbda0e-4da7-4725-8520-109cb3251f57");
    public override string EventName => "UserLogin";
}
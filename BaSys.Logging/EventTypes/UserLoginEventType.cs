using BaSys.Logging.Infrastructure;

namespace BaSys.Logging.EventTypes;
public class UserLoginEventType : EventTypeBase
{
    public override Guid Uid => new("54dbda0e-4da7-4725-8520-109cb3251f57");
    public override string EventName => "UserLogin";
}
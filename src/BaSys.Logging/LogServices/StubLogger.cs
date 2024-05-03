using BaSys.Common.Enums;
using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;

namespace BaSys.Logging.LogServices;

public class StubLogger : LoggerService
{
    public StubLogger(LoggerConfig loggerConfig, string? userUid, string? userName, string? ipAddress)
        : base(loggerConfig, userUid, userName, ipAddress)
    {
    }


    protected override void WriteInner(string? message, EventTypeLevels level, EventType eventType, string? exception = null,
        Guid? metadataUid = null, string? dataUid = null, string? dataPresentation = null)
    {
    }
}
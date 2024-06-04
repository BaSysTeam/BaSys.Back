using BaSys.Common.Enums;

namespace BaSys.Logging.Abstractions.Abstractions;

public interface ILoggerService : IDisposable
{
    void Write(string message,
        EventTypeLevels level,
        EventType eventType,
        Guid? metadataUid = null,
        string? dataUid = null,
        string? dataPresentation = null);

    void Info(string message,
        EventType eventType,
        Guid? metadataUid = null,
        string? dataUid = null,
        string? dataPresentation = null);

    void Error(string exceptionMessage,
        EventType eventType,
        Guid? metadataUid = null,
        string? dataUid = null,
        string? dataPresentation = null);
}
using BaSys.Common.Enums;

namespace BaSys.App.Features.DataObjectRecords.Commands
{
    public sealed class CreateRecordsCommand
    {
        public string KindName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;
        public string ObjectUid { get; set; } = string.Empty;
        public EventTypeLevels LogLevel { get; set; } = EventTypeLevels.Error;

        public CreateRecordsCommand(string kindName, string objectName, string objectUid, EventTypeLevels logLevel)
        {
            KindName = kindName;
            ObjectName = objectName;
            ObjectUid = objectUid;
            LogLevel = logLevel;
        }
    }
}

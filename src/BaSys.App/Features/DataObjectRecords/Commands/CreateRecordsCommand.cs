namespace BaSys.App.Features.DataObjectRecords.Commands
{
    public sealed class CreateRecordsCommand
    {
        public string KindName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;
        public string ObjectUid { get; set; } = string.Empty;

        public CreateRecordsCommand(string kindName, string objectName, string objectUid)
        {
            KindName = kindName;
            ObjectName = objectName;
            ObjectUid = objectUid;
        }
    }
}

namespace BaSys.App.Features.DataObjectRecords.Commands
{
    public sealed class DeleteRecordsCommand
    {
        public string KindName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;
        public string ObjectUid { get; set; } = string.Empty;

        public DeleteRecordsCommand(string kindName, string objectName, string objectUid)
        {
            KindName = kindName;
            ObjectName = objectName;
            ObjectUid = objectUid;
        }
    }
}

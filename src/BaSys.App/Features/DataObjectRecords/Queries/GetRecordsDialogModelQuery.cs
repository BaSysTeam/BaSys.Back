namespace BaSys.App.Features.DataObjectRecords.Queries
{
    public sealed class GetRecordsDialogModelQuery
    {
        public string KindName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;
        public string ObjectUid { get; set; } = string.Empty;

        public GetRecordsDialogModelQuery(string kindName, string objectName, string objectUid)
        {
            KindName = kindName;
            ObjectName = objectName;
            ObjectUid = objectUid;
        }
    }
}

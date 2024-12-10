namespace BaSys.App.Features.DataObjectRecords.Queries
{
    public sealed class GetRecordsQuery
    {
        public string SourceKindName { get; set; } = string.Empty;
        public string SourceObjectName { get; set; } = string.Empty;
        public string SourceObjectUid { get; set; } = string.Empty;
        public Guid DestinationObjectUid { get; set; }

        public GetRecordsQuery(string kindName, string objectName, string objectUid, Guid destinationObjectUid)
        {
            SourceKindName = kindName;
            SourceObjectName = objectName;
            SourceObjectUid = objectUid;
            DestinationObjectUid = destinationObjectUid;
        }
    }
}

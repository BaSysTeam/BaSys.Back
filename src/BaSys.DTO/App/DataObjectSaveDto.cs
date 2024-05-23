namespace BaSys.DTO.App
{
    /// <summary>
    /// DTO for Insert and Update operations.
    /// </summary>
    public sealed class DataObjectSaveDto
    {

        public Guid MetaObjectKindUid { get; set; }
        public Guid MetaObjectUid { get; set; }
        public Dictionary<string, object> Header { get; set; } = new Dictionary<string, object>();

        public DataObjectSaveDto()
        {
            
        }

        public DataObjectSaveDto(Guid kindUid, Guid objectUid, IDictionary<string, object> data)
        {
            MetaObjectKindUid = kindUid;
            MetaObjectUid = objectUid;

            foreach (var key in data.Keys)
            {
                Header[key] = data[key];
            }
        }
    }
}

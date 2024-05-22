namespace BaSys.DTO.App
{
    public sealed class DataObjectDto
    {

        public Guid MetaObjectKindUid { get; set; }
        public Guid MetaObjectUid { get; set; }
        public Dictionary<string, object> Header { get; set; } = new Dictionary<string, object>();

        public DataObjectDto()
        {
            
        }

        public DataObjectDto(Guid kindUid, Guid objectUid, IDictionary<string, object> data)
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

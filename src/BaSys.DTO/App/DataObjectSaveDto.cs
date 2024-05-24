namespace BaSys.DTO.App
{
    /// <summary>
    /// DTO for Insert and Update operations.
    /// </summary>
    public sealed class DataObjectSaveDto
    {

        public Guid MetaObjectKindUid { get; set; }
        public Guid MetaObjectUid { get; set; }
        public DataObjectDto Item { get; set; } = new DataObjectDto();

        public DataObjectSaveDto()
        {
            
        }

        public DataObjectSaveDto(Guid kindUid, Guid objectUid, IDictionary<string, object> data)
        {
            MetaObjectKindUid = kindUid;
            MetaObjectUid = objectUid;

            Item = new DataObjectDto(data);
        }
    }
}

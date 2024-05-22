namespace BaSys.DTO.App
{
    public sealed class DataObjectDto
    {
        private Dictionary<string, object> _header = new Dictionary<string, object>();

        public Guid MetaObjectKindUid { get; set; }
        public Guid MetaObjectUid { get; set; }
        public Dictionary<string, object> Header
        {
            get => _header;
            set => _header = value;
        }
    }
}

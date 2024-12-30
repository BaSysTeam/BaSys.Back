using BaSys.Host.DAL.Abstractions;
using BaSys.Metadata.Models;

namespace BaSys.Host.DAL.Helpers
{
    public sealed class AddColumnCommand: IMetaObjectChangeColumnCommand
    {
        public Guid TableUid { get; set; }
        public string TableName { get; set; } = string.Empty;
        public MetaObjectTableColumn Column { get; set; } = new MetaObjectTableColumn();

        public override string ToString()
        {
            return $"{TableName}.{Column.Name}";
        }
    }
}

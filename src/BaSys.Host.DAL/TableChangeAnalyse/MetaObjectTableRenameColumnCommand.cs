using BaSys.Host.DAL.Abstractions;

namespace BaSys.Host.DAL.TableChangeAnalyse
{
    public sealed class MetaObjectTableRenameColumnCommand : IMetaObjectChangeCommand
    {
        public Guid TableUid { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public string ColumnNameNew { get; set; } = string.Empty;
    }
}

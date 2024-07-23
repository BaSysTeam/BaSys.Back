using BaSys.FluentQueries.Models;
using BaSys.Host.DAL.Abstractions;

namespace BaSys.Host.DAL.TableChangeAnalyse
{
    public sealed class MetaObjectAlterTableCommand: IMetaObjectChangeCommand
    {
        public Guid TableUid { get; set; }
        public string TableName { get; set; } = string.Empty;
        public AlterTableModel AlterTableModel { get; set; } = new AlterTableModel();   
    }
}

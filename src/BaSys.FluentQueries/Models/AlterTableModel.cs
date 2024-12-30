using System.Collections.Generic;
using System.Linq;

namespace BaSys.FluentQueries.Models
{
    public sealed class AlterTableModel
    {
      
        public string TableName { get; set; } = string.Empty;
        public List<TableColumn> NewColumns { get; set; } = new List<TableColumn>();
        public List<string> RemovedColumns { get; set; } = new List<string>();
        public List<ChangeColumnModel> ChangedColumns { get; set; } = new List<ChangeColumnModel>();
        public List<RenameColumnModel> RenamedColumns { get; set; } = new List<RenameColumnModel>();

        public void ToLower()
        {
            TableName = TableName.ToLower();

            foreach (TableColumn column in NewColumns)
            {
                column.Name = column.Name.ToLower();    
            }

            foreach (var changeModel in ChangedColumns)
            {
                changeModel.Column.Name = changeModel.Column.Name.ToLower();
            }

            foreach (var column in RenamedColumns)
            {
                column.OldName = column.OldName.ToLower();
                column.NewName = column.NewName.ToLower();
            }

            RemovedColumns = RemovedColumns.Select(s => s.ToLower()).ToList();
        }

      
    }
}

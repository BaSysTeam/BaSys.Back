using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public sealed class AlterTableModel
    {
      
        public string TableName { get; set; } = string.Empty;
        public List<TableColumn> NewColumns { get; set; } = new List<TableColumn>();
        public List<string> RemovedColumns { get; set; } = new List<string>();

        public void ToLower()
        {
            TableName = TableName.ToLower();

            foreach (TableColumn column in NewColumns)
            {
                column.Name = column.Name.ToLower();    
            }

           RemovedColumns = RemovedColumns.Select(s => s.ToLower()).ToList();
        }

      
    }
}

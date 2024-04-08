using BaSys.FluentQueries.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public sealed class CreateTableModel
    {
        private readonly List<TableColumn> _columns = new List<TableColumn>();

        public string TableName { get; set; } = string.Empty;
        public IReadOnlyCollection<TableColumn> Columns => _columns;

        public CreateTableModel()
        {
            
        }

        public CreateTableModel(IDataModelConfiguration config)
        {
            TableName = config.TableName;
            _columns.AddRange(config.Columns);
        }

        public void AddColumn(TableColumn column) { 

            _columns.Add(column);   

        }

        public void RemoveColumn(TableColumn column) { 

            _columns.Remove(column);
        }

        public void ClearColumns()
        {
            _columns.Clear();
        }
    }
}

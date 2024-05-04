using BaSys.FluentQueries.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public abstract class DataModelConfiguration<T> : IDataModelConfiguration where T : class
    {
        private readonly List<TableColumn> _columns = new List<TableColumn>();

        public string TableName { get; set; }
        public IReadOnlyCollection<TableColumn> Columns => _columns;

        public DataModelConfiguration()
        {
            InitTableName();
            InitColumns();
        }

        public TableColumn Column(string name)
        {
            return _columns.FirstOrDefault(x=>x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void AddColumn(TableColumn column)
        {
            _columns.Add(column);
        }

        public void ClearColumns()
        {
            _columns.Clear();
        }

        public void Table(string tableName)
        {
            TableName = tableName;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Table: {TableName}");
            sb.AppendLine("Columns:");
            foreach(var column in Columns)
            {
                sb.AppendLine(column.ToString());
            }

            return sb.ToString();
        }

        private void InitTableName()
        {
            TableName = typeof(T).Name; 
        }

        private void InitColumns()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Where(p => p.GetSetMethod() != null); // Only properties with public setters

            foreach (var property in properties)
            {
                _columns.Add(new TableColumn(property.Name, property.PropertyType));
            }
        }
    }
}

using BaSys.FluentQueries.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public sealed class InsertModel
    {
        private readonly List<string> _columns = new List<string>();
        private readonly List<List<string>> _values = new List<List<string>>();
        private readonly List<QueryParameter> _parameters = new List<QueryParameter>();

        public string TableName { get; set; } = string.Empty;
        public string PrimaryKeyName { get; set; } = string.Empty;
        public bool FillValuesByColumnNames { get; set; }
        public bool ReturnId { get; set; }
        public IReadOnlyCollection<string> Columns => _columns;
        public IReadOnlyCollection<List<string>> Values => _values;
        public IReadOnlyCollection<QueryParameter> Parameters => _parameters;

        public InsertModel()
        {
           
        }

        public InsertModel(IDataModelConfiguration config)
        {
            TableName = config.TableName;
            foreach(var configColumn in config.Columns)
            {
                if (configColumn.PrimaryKey && (configColumn.DbType == DbType.Int32 || configColumn.DbType == DbType.Int64))
                    continue;

                _columns.Add(configColumn.Name);
            }
        }

        public void AddColumn(string columnName)
        {
            _columns.Add(columnName);
        }

        public void RemoveColumn(string columnName)
        {
            _columns.Remove(columnName);
        }

        public void ClearColumns()
        {
            _columns.Clear();
        }

        public void AddParameter(string parameterName, object value)
        {

            var newParameter = new QueryParameter(parameterName, value);
            _parameters.Add(newParameter);

            var valuesRow = LastValuesRow();
            valuesRow.Add(parameterName);

        }

        public void RemoveParameter(string parameterName)
        {
            var parameter = _parameters.FirstOrDefault(x => x.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
            if (parameter != null)
            {
                _parameters.Remove(parameter);
            }
        }

        public void ClearParameters()
        {
            _parameters.Clear();
        }

        public void AddValuesRow()
        {
            _values.Add(new List<string>());    
        }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(TableName))
                throw new InvalidOperationException($"{GetType().Name}. Table name cannot be null or whitespace.");

            if (ReturnId)
            {
                if (string.IsNullOrEmpty(PrimaryKeyName))
                    throw new InvalidOperationException($"{GetType().Name}. Primary key name cannot be null or whitespace if flag ReturnId is true.");
            }

            return true;
        }

        private List<string> LastValuesRow()
        {
            if (_values.Count == 0)
                AddValuesRow();

            return _values.LastOrDefault();
        }
    }
}

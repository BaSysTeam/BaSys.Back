using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public sealed class InsertModel
    {
        private readonly List<string> _columns;
        private readonly List<List<string>> _values;
        private readonly List<QueryParameter> _parameters;

        public string TableName { get; set; } = string.Empty;
        public IReadOnlyCollection<string> Columns => _columns;
        public IReadOnlyCollection<List<string>> Values => _values;
        public IReadOnlyCollection<QueryParameter> Parameters => _parameters;

        public InsertModel()
        {
            _columns = new List<string>();
            _values = new List<List<string>>();
            _parameters = new List<QueryParameter>();
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

        private List<string> LastValuesRow()
        {
            if (_values.Count == 0)
                AddValuesRow();

            return _values.LastOrDefault();
        }
    }
}

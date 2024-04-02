using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public sealed class DeleteModel
    {
        private readonly List<QueryParameter> _parameters;

        public string TableName { get; set; } = string.Empty;
        public string WhereExpression { get; set; } = string.Empty;
        public IReadOnlyCollection<QueryParameter> Parameters => _parameters;

        public DeleteModel()
        {
            _parameters = new List<QueryParameter>();
        }

        public void WhereAnd(string whereExpression)
        {
            ConcatenateWhereExpression(whereExpression, "AND");
        }

        public void WhereOr(string whereExpression)
        {
            ConcatenateWhereExpression(whereExpression, "OR");
        }

        public void AddParameter(string parameterName, object value)
        {
            var newParameter = new QueryParameter(parameterName, value);
            _parameters.Add(newParameter);
        }

        public void AddParameter(string parameterName, object value, DbType dbType)
        {
            var newParameter = new QueryParameter(parameterName, value, dbType);
            _parameters.Add(newParameter);
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

        private void ConcatenateWhereExpression(string whereExpression, string logicOperator)
        {
            if (string.IsNullOrEmpty(WhereExpression))
            {
                WhereExpression = whereExpression;
            }
            else
            {
                WhereExpression += $" {logicOperator} " + whereExpression;
            }
        }

    }
}

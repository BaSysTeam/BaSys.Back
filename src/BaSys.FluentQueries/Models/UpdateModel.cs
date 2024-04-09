using BaSys.FluentQueries.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public sealed class UpdateModel
    {
        private readonly List<QueryParameter> _parameters = new List<QueryParameter>();
        private readonly Dictionary<string, string> _setExpressions = new Dictionary<string, string>();

        public string TableName { get; set; } = string.Empty;
        public string WhereExpression { get; set; } = string.Empty;

        public IReadOnlyCollection<QueryParameter> Parameters => _parameters;
        public IDictionary<string, string> SetExpressions => _setExpressions;

        public UpdateModel()
        {
            
        }

        public UpdateModel(IDataModelConfiguration config)
        {
            TableName = config.TableName;
            foreach(var configColumn in config.Columns)
            {
                if (configColumn.PrimaryKey)
                    continue;

                _setExpressions.Add(configColumn.Name, string.Empty);
            }
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


        public void LoadSetExpressions(Dictionary<string, string> setExpressions)
        {
            foreach (var kvp in setExpressions)
            {
                _setExpressions[kvp.Key] = kvp.Value;
            }
        }

        public void AddSetExpression(string leftExpression, string rightExpression)
        {
            _setExpressions[leftExpression] = rightExpression;
        }

        public bool RemoveSetExpression(string leftExpression)
        {
            var result = _setExpressions.Remove(leftExpression);

            return result;
        }

        public void ClearSetExpressions()
        {
            _setExpressions.Clear();
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

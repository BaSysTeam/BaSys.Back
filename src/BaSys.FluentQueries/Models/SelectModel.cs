using BaSys.FluentQueries.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BaSys.FluentQueries.Models
{
    public sealed class SelectModel
    {
        private readonly List<string> _selectExpressions = new List<string>();
        private readonly List<SelectFieldModel> _fields = new List<SelectFieldModel>();
        private readonly List<QueryParameter> _parameters = new List<QueryParameter>();
        private readonly List<JoinModel> _joinExpressions = new List<JoinModel>();

        public int Top { get; set; }
        public string FromExpression { get; set; } = string.Empty;
        public string WhereExpression { get; set; } = string.Empty;
        public string OrderByExpression { get; set; } = string.Empty;

        public IReadOnlyCollection<string> SelectExpressions => _selectExpressions;
        public IReadOnlyCollection<QueryParameter> Parameters => _parameters;
        public IReadOnlyCollection<JoinModel> JoinExpressions => _joinExpressions;
        public IReadOnlyCollection<SelectFieldModel> Fields => _fields;

        public SelectModel()
        {
            _selectExpressions = new List<string>();
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

        public void AddSelectExpression(string selectExpression)
        {
            _selectExpressions.Add(selectExpression);
        }

        public void RemoveSelectExpression(string selectExpression)
        {
            _selectExpressions.Remove(selectExpression);
        }

        public void RemoveSelectExpression(int i)
        {
            _selectExpressions.RemoveAt(i);
        }

        public void ClearSelectExpressions()
        {
            _selectExpressions.Clear();
        }

        public void AddJoin(JoinKinds joinKind, string tableName, IEnumerable<ConditionModel> joinCondition)
        {
            var joinModel = new JoinModel(joinKind, tableName, joinCondition);

            _joinExpressions.Add(joinModel);
        }

        public void ClearJoins()
        {
            _joinExpressions.Clear();
        }

        public void AddField(SelectFieldModel field)
        {
            _fields.Add(field);
        }

        public void ClearFields()
        {
            _fields.Clear();
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

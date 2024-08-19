using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class SelectScriptGenerator : ScriptGeneratorBase
    {
        private readonly SelectModel _model;

        public SelectScriptGenerator(SelectModel model, SqlDialectKinds sqlDialect) : base(sqlDialect)
        {
            _model = model;
        }

        public IQuery Build()
        {
            var query = new Query();

            Append("SELECT ");
            if (_model.Top > 0 && _sqlDialect == SqlDialectKinds.MsSql)
            {
                _sb.Append($"TOP {_model.Top} ");
            }

            var n = 1;
            foreach (var selectExpression in _model.SelectExpressions)
            {
                if (n > 1)
                    AppendLine(", ");

                Append(selectExpression);

                n++;
            }

            // Add Select Fields.
            foreach(var selectField in _model.Fields)
            {
                AppendLineIf(", ", n > 1);

                var tableName = string.IsNullOrWhiteSpace(selectField.TableName) ? _model.FromExpression : selectField.TableName;
                AppendName(tableName);
                Append('.');
                AppendName(selectField.FieldName);

                if (!string.IsNullOrEmpty(selectField.Alias))
                {
                    Append(" AS ");
                    Append(selectField.Alias);
                }

                n++;
            }
            AppendLine();

            Append("FROM ");
            AppendName(_model.FromExpression);

            // JOINS
            if (_model.JoinExpressions.Any())
            {
                foreach (var joinModel in _model.JoinExpressions)
                {
                    BuildJoinExpression(joinModel);
                }
            }

            if (!string.IsNullOrEmpty(_model.WhereExpression))
            {
                AppendLine(string.Empty);
                Append("WHERE ");
                Append(_model.WhereExpression);
            }
            if (!string.IsNullOrEmpty(_model.OrderByExpression))
            {
                AppendLine();
                Append("ORDER BY ");
                Append(_model.OrderByExpression);

            }
            if (_model.Top > 0 && _sqlDialect == SqlDialectKinds.PgSql)
            {
                AppendLine();
                Append($"LIMIT {_model.Top}");
            }
            Append(";");


            query.Text = _sb.ToString();
            query.AddParameters(_model.Parameters);

            return query;
        }

        private void BuildJoinExpression(JoinModel joinModel)
        {
            AppendLine();
            Append(GetJoinKind(joinModel.JoinKind));
            Append(' ');
            AppendLine("JOIN");

            AppendName(joinModel.TableName);
            Append(" ON ");

            var n = 1;
            foreach (var condtion in joinModel.JoinConditions)
            {
                if (n > 1)
                {
                    Append(' ');
                    Append(GetLogicalOperator(condtion.LogicalOperator));
                }

                AppendName(condtion.LeftTable);
                Append(".");
                AppendName(condtion.LeftField);

                Append(' ');
                Append(GetComparisionOperator(condtion.ComparisionOperator));
                Append(' ');

                AppendName(condtion.RightTable);
                Append(".");
                AppendName(condtion.RightField);

                n++;
            }

        }

        private string GetLogicalOperator(LogicalOperators logicalOperator)
        {
            switch (logicalOperator)
            {
                case LogicalOperators.AND:
                    return "AND";
                case LogicalOperators.OR:
                    return "OR";
                default:
                    throw new NotSupportedException($"Logical {logicalOperator} is not supported");
            }
        }

        private string GetJoinKind(JoinKinds joinKind)
        {

            switch (joinKind)
            {
                case JoinKinds.Inner:
                    return "INNER";
                case JoinKinds.Left:
                    return "LEFT";
                case JoinKinds.Right:
                    return "RIGHT";
                case JoinKinds.Full:
                    return "FULL";
                default:
                    throw new NotSupportedException($"Join kind {joinKind} is not supported");
            }

        }

        private string GetComparisionOperator(ComparisionOperators comparisionOperator)
        {
            switch (comparisionOperator)
            {
                case ComparisionOperators.Equal:
                    return "=";
                case ComparisionOperators.NotEqual:
                    return "!=";
                case ComparisionOperators.Greater:
                    return ">";
                case ComparisionOperators.GreaterOrEqual:
                    return ">=";
                case ComparisionOperators.Less:
                    return "<";
                case ComparisionOperators.LessOrEqual:
                    return "<=";
                default:
                    throw new NotSupportedException($"Comparision operator {comparisionOperator} is not supported");
            }
        }


    }
}

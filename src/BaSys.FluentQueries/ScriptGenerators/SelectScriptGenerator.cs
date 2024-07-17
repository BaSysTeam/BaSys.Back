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

            _sb.Append("SELECT ");
            if (_model.Top > 0 && _sqlDialect == SqlDialectKinds.MsSql)
            {
                _sb.Append($"TOP {_model.Top} ");
            }

            var n = 1;
            foreach (var selectExpression in _model.SelectExpressions)
            {
                if (n > 1)
                    _sb.AppendLine(", ");

                _sb.Append(selectExpression);

                n++;
            }
            _sb.AppendLine();

            _sb.Append("FROM ");
            _sb.Append(_model.FromExpression);

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
                _sb.AppendLine();
                _sb.Append("WHERE ");
                _sb.Append(_model.WhereExpression);
            }
            if (!string.IsNullOrEmpty(_model.OrderByExpression))
            {
                _sb.AppendLine();
                _sb.Append("ORDER BY ");
                _sb.Append(_model.OrderByExpression);

            }
            if (_model.Top > 0 && _sqlDialect == SqlDialectKinds.PgSql)
            {
                _sb.AppendLine();
                _sb.Append($"LIMIT {_model.Top}");
            }
            _sb.Append(";");


            query.Text = _sb.ToString();
            query.AddParameters(_model.Parameters);

            return query;
        }

        private void BuildJoinExpression(JoinModel joinModel)
        {
            _sb.AppendLine();
            _sb.Append(GetJoinKind(joinModel.JoinKind));
            _sb.Append(' ');
            _sb.AppendLine("JOIN");

            AppendName(joinModel.TableName);
            _sb.Append(" ON ");

            var n = 1;
            foreach (var condtion in joinModel.JoinConditions)
            {
                if (n > 1)
                {
                    _sb.Append(' ');
                    _sb.Append(GetLogicalOperator(condtion.LogicalOperator));
                }

                AppendName(condtion.LeftTable);
                _sb.Append(".");
                AppendName(condtion.LeftField);

                _sb.Append(' ');
                _sb.Append(GetComparisionOperator(condtion.ComparisionOperator));
                _sb.Append(' ');

                AppendName(condtion.RightTable);
                _sb.Append(".");
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

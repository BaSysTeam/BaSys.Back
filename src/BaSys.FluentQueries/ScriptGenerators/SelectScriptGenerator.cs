﻿using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
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

            var sb = new StringBuilder();
            sb.Append("SELECT ");
            if (_model.Top > 0 && _sqlDialect == SqlDialectKinds.MsSql)
            {
                sb.Append($"TOP {_model.Top} ");
            }

            var n = 1;
            foreach(var selectExpression  in _model.SelectExpressions)
            {
                if(n>1)
                    sb.AppendLine(", ");

                sb.Append(selectExpression);

                n++;
            }
            sb.AppendLine();

            sb.Append("FROM ");
            sb.Append(_model.FromExpression);
           

            if (!string.IsNullOrEmpty(_model.WhereExpression))
            {
                sb.AppendLine();
                sb.Append("WHERE ");
                sb.Append(_model.WhereExpression);
            }
            if (!string.IsNullOrEmpty(_model.OrderByExpression))
            {
                sb.AppendLine();
                sb.Append("ORDER BY ");
                sb.Append(_model.OrderByExpression);

            }
            if (_model.Top > 0 && _sqlDialect == SqlDialectKinds.PgSql)
            {
                sb.AppendLine();
                sb.Append($"LIMIT {_model.Top}");
            }
            sb.Append(";");


            query.Text = sb.ToString();
            query.AddParameters(_model.Parameters);

            return query;
        }
    }
}

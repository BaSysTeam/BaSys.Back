using BaSys.FluentQueries.Abstractions;
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
            sb.Append(";");


            query.Text = sb.ToString();
            return query;
        }
    }
}

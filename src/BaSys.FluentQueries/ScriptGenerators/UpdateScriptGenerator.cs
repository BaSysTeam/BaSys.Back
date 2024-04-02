using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class UpdateScriptGenerator: ScriptGeneratorBase
    {
        private readonly UpdateModel _model;

        public UpdateScriptGenerator(UpdateModel model, SqlDialectKinds sqlDialect):base(sqlDialect)
        {
            _model = model;
        }

        public IQuery Build()
        {
            var query = new Query();

            var sb = new StringBuilder();

            sb.AppendLine($"UPDATE {_model.TableName}");
            sb.AppendLine("SET");

            var n = 1;
            foreach(var kvp in _model.SetExpressions)
            {
                if(n > 1)
                {
                    sb.AppendLine(", ");
                }

                var leftExpression = kvp.Key;
                var rightExpression = kvp.Value;

                if (string.IsNullOrEmpty(rightExpression))
                {
                    rightExpression = "@" + leftExpression;
                }

                sb.Append($"{leftExpression} = {rightExpression}");
                n++;
            }
            sb.AppendLine();
          

            // Where
            sb.AppendLine("WHERE");
            sb.Append(_model.WhereExpression);
            sb.Append(";");

            query.Text = sb.ToString();
            return query;
        }

    }
}

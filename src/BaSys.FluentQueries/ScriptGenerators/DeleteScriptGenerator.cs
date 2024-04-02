using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class DeleteScriptGenerator : ScriptGeneratorBase
    {
        private readonly DeleteModel _model;

        public DeleteScriptGenerator(DeleteModel model, SqlDialectKinds sqlDialect) : base(sqlDialect)
        {
            _model = model;
        }

        public IQuery Build()
        {
            var query = new Query();

            var sb = new StringBuilder();

            sb.Append($"DELETE FROM {_model.TableName}");

            sb.AppendLine();
            sb.Append("WHERE ");
            sb.Append(_model.WhereExpression);

            sb.Append(";");

            query.Text = sb.ToString();
            query.AddParameters(_model.Parameters);

            return query;
        }
    }
}

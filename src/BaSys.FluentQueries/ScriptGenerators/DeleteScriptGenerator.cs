using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;

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

            Append($"DELETE FROM ");
            AppendName(_model.TableName); 

            AppendLine();
            Append("WHERE ");
            Append(_model.WhereExpression);

            Append(";");

            query.Text = _sb.ToString();
            query.AddParameters(_model.Parameters);

            return query;
        }
    }
}

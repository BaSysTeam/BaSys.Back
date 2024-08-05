using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    public sealed class BulkCopyPgSqlScriptGenerator : ScriptGeneratorBase
    {
        private InsertModel _model;

        public BulkCopyPgSqlScriptGenerator(InsertModel model, SqlDialectKinds sqlDialect) : base(sqlDialect)
        {
            _model = model;
        }

        public IQuery Build()
        {

            Append($"COPY ");
            AppendName(_model.TableName);
            Append(" (");
            var n = 1;
            foreach (var column in _model.Columns)
            {
                AppendIf(", ", n > 1);
                AppendName(column);
                n++;
            }
            Append(") FROM STDIN (FORMAT BINARY)");

            var query = new Query();
            query.Text = _sb.ToString();
            return query;
        }
    }
}

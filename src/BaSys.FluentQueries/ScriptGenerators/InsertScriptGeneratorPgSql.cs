using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class InsertScriptGeneratorPgSql : InsertScriptGeneratorBase
    {
        public InsertScriptGeneratorPgSql(InsertModel model) : base(SqlDialectKinds.PgSql, model)
        {

        }

        protected override void AppendReturning()
        {
            if (_model.ReturnId)
            {
                _sb.AppendLine();
                _sb.Append($"RETURNING {_model.PrimaryKeyName}");
            }
        }
    }
}

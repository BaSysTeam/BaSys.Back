using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class InsertScriptGeneratorMsSql : InsertScriptGeneratorBase
    {
        public InsertScriptGeneratorMsSql(InsertModel model) : base(SqlDialectKinds.MsSql, model)
        {

        }

        protected override void AppendOutput()
        {
            if (_model.ReturnId)
            {
                _sb.AppendLine($"OUTPUT INSERTED.{_model.PrimaryKeyName}");
            }
        }
    }
}

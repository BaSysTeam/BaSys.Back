using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    public abstract class AlterTableScriptGeneratorBase: ScriptGeneratorBase, IQueryBuilder
    {
        protected readonly AlterTableModel _model;

        protected AlterTableScriptGeneratorBase(SqlDialectKinds sqlDialect, AlterTableModel model):base(sqlDialect)
        {
            _model = model;
        }

        public virtual IQuery Build()
        {
            _model.ToLower();
            var query = new Query();

            _sb.Clear();
            _sb.Append($"ALTER TABLE ");
            AppendName(_model.TableName);
            _sb.AppendLine();

            query.Text = _sb.ToString(); 

            return query;
        }
    }
}

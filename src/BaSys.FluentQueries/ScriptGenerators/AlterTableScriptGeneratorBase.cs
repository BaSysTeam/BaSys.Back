using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Data;
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

        protected abstract string GetDataType(DbType dbType, int stringLenght);

        public virtual IQuery Build()
        {
            _model.ToLower();
            var query = new Query();

            _sb.Clear();
            _sb.Append($"ALTER TABLE ");
            AppendName(_model.TableName);
            _sb.AppendLine();

            var n = 1;
            foreach (var column in _model.NewColumns)
            {
                if (n > 1)
                    _sb.AppendLine(",");

                AddColumnQuery(column, n);

                n++;
            }
            _sb.Append(';');

            query.Text = _sb.ToString(); 

            return query;
        }

        private void AddColumnQuery(TableColumn column, int counter)
        {
            switch (_sqlDialect)
            {
                case SqlDialectKinds.MsSql:
                    if (counter == 1)
                      _sb.Append("ADD ");

                    break;
                case SqlDialectKinds.PgSql:
                    _sb.Append("ADD COLUMN ");
                    break;
                default:
                    throw new NotImplementedException();
            }

            AppendName(column.Name);
            _sb.Append(' ');
            _sb.Append(GetDataType(column.DbType, column.StringLength));

            if (column.Required)
            {
                _sb.Append(" NOT NULL");
            }
            else
            {
                _sb.Append(" NULL");
            }

            if (column.Unique)
            {
                _sb.Append(" UNIQUE");
            }

        }
    }
}

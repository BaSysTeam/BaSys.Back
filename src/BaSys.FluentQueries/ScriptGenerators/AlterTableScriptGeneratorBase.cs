using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    public abstract class AlterTableScriptGeneratorBase : ScriptGeneratorBase, IQueryBuilder
    {
        protected readonly AlterTableModel _model;

        protected AlterTableScriptGeneratorBase(SqlDialectKinds sqlDialect, AlterTableModel model) : base(sqlDialect)
        {
            _model = model;
        }

        protected abstract string GetDataType(DbType dbType, int stringLenght);

        public virtual IQuery Build()
        {
            _model.ToLower();
            var query = new Query();

            _sb.Clear();
            var n = 1;

            foreach (var column in _model.NewColumns)
            {
                AddColumnQuery(column, n);
                n++;
            }

            foreach (var column in _model.ChangedColumns)
            {
                AlterColumnQuery(column, n);
                n++;
            }

            foreach (var columnName in _model.RemovedColumns)
            {
                DropColumnQuery(columnName, n);
                n++;
            }

            foreach (var renameModel in _model.RenamedColumns)
            {
                RenameColumnQuery(renameModel, n);
                n++;
            }

            query.Text = _sb.ToString();

            return query;
        }

        private void AlterColumnQuery(TableColumn column, int counter)
        {
            if (counter > 1)
                AppendLine("");

            Append("ALTER TABLE ");
            AppendName(_model.TableName);
            Append(' ');

            var dataTypeStr = GetDataType(column.DbType, column.StringLength);

            _sb.Append("ALTER COLUMN ");
            AppendName(column.Name);
            _sb.Append(' ');
            if (_sqlDialect == SqlDialectKinds.PgSql)
            {
                _sb.Append("TYPE ");
            }
            _sb.Append(dataTypeStr);

            if (_sqlDialect == SqlDialectKinds.MsSql)
            {
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

            if (_sqlDialect == SqlDialectKinds.PgSql)
            {
                _sb.Append(' ');
                _sb.Append("USING ");
                AppendName(column.Name);
                _sb.Append("::");
                _sb.Append(dataTypeStr);

            }

            Append(';');
        }

        private void AddColumnQuery(TableColumn column, int counter)
        {
            if (counter > 1)
                AppendLine("");

            Append("ALTER TABLE ");
            AppendName(_model.TableName);
            Append(' ');

            switch (_sqlDialect)
            {
                case SqlDialectKinds.MsSql:
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
            _sb.Append(';');
        }

        private void DropColumnQuery(string columnName, int counter)
        {
            if (counter > 1)
                AppendLine("");

            Append("ALTER TABLE ");
            AppendName(_model.TableName);
            Append(' ');

            Append("DROP COLUMN ");
            AppendName(columnName);

            Append(';');
        }

        private void RenameColumnQuery(RenameColumnModel renameModel, int counter)
        {
            if (counter > 1)
                AppendLine("");

            if (_sqlDialect == SqlDialectKinds.MsSql)
            {
                // Rename column in MS SQL.
                var renameExpression = $"EXEC sp_rename '{_model.TableName}.{renameModel.OldName}', '{renameModel.NewName}', 'COLUMN';";
                _sb.Append(renameExpression);
            }
            else if (_sqlDialect == SqlDialectKinds.PgSql)
            {
                Append("ALTER TABLE ");
                AppendName(_model.TableName);
                Append(' ');

                _sb.Append("RENAME COLUMN ");
                AppendName(renameModel.OldName);
                _sb.Append(" TO ");
                AppendName(renameModel.NewName);

                Append(';');
            }
        }

      
    }
}

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
            var n = 1;

            if (IsAlterTableExpressionNecessary())
            {
                _sb.Append($"ALTER TABLE ");
                AppendName(_model.TableName);
                _sb.AppendLine();

               
                foreach (var column in _model.NewColumns)
                {
                    if (n > 1)
                        _sb.AppendLine(",");

                    AddColumnQuery(column, n);

                    n++;
                }

                foreach (var column in _model.ChangedColumns)
                {
                    if (n > 1)
                        _sb.AppendLine(",");

                    AlterColumnQuery(column, n);

                    n++;
                }

                foreach (var columnName in _model.RemovedColumns)
                {
                    if (n > 1)
                        _sb.AppendLine(",");

                    _sb.Append("DROP COLUMN ");
                    AppendName(columnName);

                    n++;
                }

                if (_sqlDialect == SqlDialectKinds.PgSql)
                {
                    // Rename column in PG SQL.
                    foreach (var renameModel in _model.RenamedColumns)
                    {
                        if (n > 1)
                            _sb.AppendLine(",");

                        _sb.Append("RENAME COLUMN ");
                        AppendName(renameModel.OldName);
                        _sb.Append(" TO ");
                        AppendName(renameModel.NewName);

                        n++;
                    }
                }

                _sb.Append(';');
            }

            if (_sqlDialect == SqlDialectKinds.MsSql)
            {
                // Rename column in MS SQL.
                foreach (var renameModel in _model.RenamedColumns)
                {
                    if (n > 1)
                        _sb.AppendLine("");
                    var renameExpression = $"EXEC sp_rename '{_model.TableName}.{renameModel.OldName}', '{renameModel.NewName}', 'COLUMN';";
                    _sb.Append(renameExpression);
                }
            }

            query.Text = _sb.ToString(); 

            return query;
        }

        private void AlterColumnQuery(TableColumn column, int counter)
        {
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

        private bool IsAlterTableExpressionNecessary()
        {
            var result = false;

            if (_sqlDialect == SqlDialectKinds.PgSql)
            {
                result = true;
            }
            else if (_sqlDialect == SqlDialectKinds.MsSql)
            {
                result = _model.NewColumns.Any() 
                    || _model.RemovedColumns.Any() 
                    || _model.ChangedColumns.Any();
            }

            return result;
        }
    }
}

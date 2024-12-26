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
                n = AddColumnQuery(column, n);
            }

            foreach (var changeModel in _model.ChangedColumns)
            {
               n = AlterColumnQuery(changeModel, n);
            }

            foreach (var columnName in _model.RemovedColumns)
            {
               n = DropColumnQuery(columnName, n);
            }

            foreach (var renameModel in _model.RenamedColumns)
            {
               n = RenameColumnQuery(renameModel, n);
            }

            query.Text = _sb.ToString();

            return query;
        }

        private int ChangeDataTypeQuery(TableColumn column, int counter)
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

            return counter + 1;
        }

        private int ChangeRequiredQuery(TableColumn column, int counter)
        {
            if (counter > 1)
                AppendLine("");

            Append("ALTER TABLE ");
            AppendName(_model.TableName);
            Append(' ');

            _sb.Append("ALTER COLUMN ");
            AppendName(column.Name);
            if (column.Required)
            {
                _sb.Append(" SET NOT NULL");
            }
            else
            {
                _sb.Append(" DROP NOT NULL");
            }

            Append(';');

            return counter + 1;
        }

        private int AlterColumnQuery(ChangeColumnModel changeModel, int counter)
        {
            var result = counter;
            var column = changeModel.Column;

            if (changeModel.DataTypeChanged)
            {
                result = ChangeDataTypeQuery(column, result);
            }

            if (changeModel.RequiredChanged)
            {
                if (_sqlDialect == SqlDialectKinds.MsSql && !changeModel.DataTypeChanged)
                {
                    result = ChangeDataTypeQuery(column, result);
                }
                else if (_sqlDialect == SqlDialectKinds.PgSql)
                {
                    result = ChangeRequiredQuery(column, result);
                }
            }

            if (changeModel.UniqueChanged)
            {
                if (column.Unique)
                {
                    result = AddUniqueConstraintQuery(_model.TableName, column, result);
                }
                else
                {
                    result = DropUniqueConstraintQuery(column, result);
                }
            }


            return result;
        }

        private int AddColumnQuery(TableColumn column, int counter)
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

            return counter + 1;
        }

        private int DropColumnQuery(string columnName, int counter)
        {
            if (counter > 1)
                AppendLine("");

            Append("ALTER TABLE ");
            AppendName(_model.TableName);
            Append(' ');

            Append("DROP COLUMN ");
            AppendName(columnName);

            Append(';');

            return counter + 1;
        }

        private int RenameColumnQuery(RenameColumnModel renameModel, int counter)
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

            return counter + 1;
        }

       

        private int DropUniqueConstraintQuery(TableColumn column, int counter)
        {
            if (counter > 1)
                AppendLine("");

            Append("ALTER TABLE ");
            AppendName(_model.TableName);
            Append(' ');

            var constraintName = UniqueConstraintName(_model.TableName, column.Name);
            Append("DROP CONSTRAINT ");
            Append(constraintName);
            Append(';');

            return counter + 1;
        }

       

    }
}

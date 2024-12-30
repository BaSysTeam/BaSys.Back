using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System.Data;
using System.Linq;

namespace BaSys.FluentQueries.ScriptGenerators
{
    public abstract class CreateTableScriptGeneratorBase : ScriptGeneratorBase, IQueryBuilder
    {
        protected readonly CreateTableModel _model;

        protected CreateTableScriptGeneratorBase(SqlDialectKinds sqlDialect, CreateTableModel model):base(sqlDialect)
        {
            _model = model;
        }

        protected abstract string GetDataType(DbType dbType, int stringLenght);
        protected abstract string GeneratePrimaryKey(TableColumn column);

        protected virtual void AddColumnQuery(TableColumn column)
        {

            if (column.PrimaryKey)
            {
                var primaryKeyExpression = GeneratePrimaryKey(column);
                Append(primaryKeyExpression);
            }
            else
            {
                Append(column.Name);
                Append(' ');
                Append(GetDataType(column.DbType, column.StringLength));

                if (column.Required)
                {
                    Append(" NOT NULL");
                }
                else
                {
                    Append(" NULL");
                }

            }

          
        }

        public virtual IQuery Build()
        {
            _model.TableName = _model.TableName.ToLower();
            foreach(var column in _model.Columns)
            {
                column.Name = column.Name.ToLower();
            }

            var query = new Query();

            // Create table.
            AppendLine($"CREATE TABLE {_model.TableName} (");
            var n = 1;
            foreach (var column in _model.Columns)
            {
                if (n > 1)
                    AppendLine(",");
                AddColumnQuery(column);

                n++;
            }
            AppendLine();
            Append(");");

            // Add unique constraints.
            foreach(var column in _model.Columns.Where(x => x.Unique))
            {
               n = AddUniqueConstraintQuery(_model.TableName, column, n);
            }


            query.Text = _sb.ToString();
            return query;
        }
    }
}

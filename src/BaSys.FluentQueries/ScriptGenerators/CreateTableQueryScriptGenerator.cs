using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    public abstract class CreateTableQueryScriptGenerator : IQueryBuilder
    {
        protected readonly CreateTableModel _model;

        protected CreateTableQueryScriptGenerator(CreateTableModel model)
        {
            _model = model;
        }

        protected abstract string GetDataType(DbType dbType, int stringLenght);

        protected virtual void AddColumnQuery(StringBuilder sb, TableColumn column)
        {
            sb.Append(column.Name);
            sb.Append(' ');
            sb.Append(GetDataType(column.DbType, column.StringLength));

            if (column.PrimaryKey)
            {
                sb.Append(" PRIMARY KEY");
                return;
            }

            if (column.Required)
            {
                sb.Append(" NOT NULL");
            }
            else
            {
                sb.Append(" NULL");
            }
        }

        public virtual IQuery Build()
        {
            var query = new Query();

            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE {_model.TableName} (");
            var n = 1;
            foreach (var column in _model.Columns)
            {
                if (n > 1)
                    sb.AppendLine(",");
                AddColumnQuery(sb, column);

                n++;
            }
            sb.AppendLine();
            sb.Append(");");

            query.Text = sb.ToString();
            return query;
        }
    }
}

using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.ScriptGenerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public sealed class InsertBuilder
    {
        private readonly InsertModel _model;

        public InsertBuilder()
        {
            _model = new InsertModel();
        }

        public InsertBuilder(InsertModel model)
        {
            _model = model;
        }

        public InsertBuilder Table(string tableName)
        {
            _model.TableName = tableName;
            return this;
        }

        public InsertBuilder Column(string columnName)
        {
            _model.AddColumn(columnName);
            return this;
        }

        public InsertBuilder Value(string parameterName)
        {
            _model.AddParameter(parameterName, null);
            return this;
        }

        public InsertBuilder Value(string parameterName, object value)
        {
            _model.AddParameter(parameterName, value);
            return this;
        }

        public IQuery Query(SqlDialectKinds dbKind)
        {
            // Validate();

            IQuery query = null;

            switch (dbKind)
            {
                case SqlDialectKinds.MsSql:
                case SqlDialectKinds.PgSql:
                    query = GenerateScript(dbKind);
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {dbKind}.");

            }

            return query;

        }

        private IQuery GenerateScript(SqlDialectKinds dbKind)
        {
            var wOpen = NameWrapperOpen(dbKind);
            var wClosed = NameWrapperClosed(dbKind); 

            IQuery query = new Query();

            var sb = new StringBuilder();
            sb.AppendLine($"INSERT INTO {wOpen}{_model.TableName}{wClosed}");

            sb.Append("(");
            var n = 1;
            foreach(var column in _model.Columns)
            {
                if (n > 1)
                    sb.Append(", ");
                sb.Append(wOpen);
                sb.Append(column);
                sb.Append(wClosed);
                n++;
            }
            sb.AppendLine(")");
            sb.AppendLine("VALUES");

            foreach(var row in _model.Values)
            {
                n = 1;
                sb.Append('(');
                foreach (var value in row) {

                    if (n > 1)
                        sb.Append(", ");

                    sb.Append('@');
                    sb.Append(value);
                    n++;
                }
                sb.AppendLine(")");
            }
            sb.Append(";");

            query.Text = sb.ToString();

            return query;
        }

        private char NameWrapperOpen(SqlDialectKinds dialectKind)
        {
            switch (dialectKind)
            {
                case SqlDialectKinds.MsSql:
                    return '[';
                case SqlDialectKinds.PgSql:
                    return '"';
                default:
                    throw new NotImplementedException();
            }
        }

        private char NameWrapperClosed(SqlDialectKinds dialectKind)
        {
            switch (dialectKind)
            {
                case SqlDialectKinds.MsSql:
                    return ']';
                case SqlDialectKinds.PgSql:
                    return '"';
                default:
                    throw new NotImplementedException();
            }
        }

        public static InsertBuilder Make()
        {
            return new InsertBuilder();
        }
    }
}

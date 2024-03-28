using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    internal class InsertScriptGenerator: IQueryBuilder
    {
        private readonly InsertModel _model;
        private SqlDialectKinds _sqlDialect;

        public InsertScriptGenerator(InsertModel model, SqlDialectKinds sqlDialect)
        {
            _model = model;
            _sqlDialect = sqlDialect;
        }

        public IQuery Build()
        {
            var query = GenerateScript(_sqlDialect);

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
            foreach (var column in _model.Columns)
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

            if (_model.FillValuesByColumnNames)
            {
                sb.Append("(");
                n = 1;
                foreach (var column in _model.Columns)
                {
                    if (n > 1)
                        sb.Append(", ");
                   
                    sb.Append('@');
                    sb.Append(column);
                    n++;
                }
                sb.Append(")");
            }
            else
            {
                foreach (var row in _model.Values)
                {
                    n = 1;
                    sb.Append('(');
                    foreach (var value in row)
                    {

                        if (n > 1)
                            sb.Append(", ");

                        sb.Append('@');
                        sb.Append(value);
                        n++;
                    }
                    sb.Append(")");
                }
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
    }
}

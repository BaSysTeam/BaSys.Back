using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public sealed class TruncateTableBuilder
    {
        private string _tableName;

        public TruncateTableBuilder Table(string tableName)
        {
            _tableName = tableName;
            return this;
        }

        public IQuery Query(SqlDialectKinds dbKind)
        {
            Validate();

            IQuery query = new Query();

            switch (dbKind)
            {
                case SqlDialectKinds.MsSql:
                    query.Text = $"TRUNCATE TABLE {_tableName};";
                    break;
                case SqlDialectKinds.PgSql:
                    query.Text = $"TRUNCATE {_tableName};";
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {dbKind}.");

            }

            return query;

        }

        public static TruncateTableBuilder Make()
        {
            return new TruncateTableBuilder();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(_tableName))
            {
                throw new ArgumentException($"{GetType().Name}. Table name is empty.");
            }
        }
    }
}

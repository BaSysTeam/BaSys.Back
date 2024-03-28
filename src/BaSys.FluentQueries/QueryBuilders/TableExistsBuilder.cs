using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public sealed class TableExistsBuilder
    {
        private string _tableName;

        public TableExistsBuilder Table(string tableName)
        {
            _tableName = tableName;
            return this;
        }

        public IQuery Query(SqlDialectKinds dialectKind)
        {
            var query = new Query();

            _tableName = _tableName.ToLower();

            switch (dialectKind)
            {
                case SqlDialectKinds.MsSql:
                    query.Text = $"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{_tableName}')"
                        + "SELECT 1 AS Exists ELSE SELECT 0 AS Exists";

                    break;
                case SqlDialectKinds.PgSql:
                    query.Text = $"SELECT CASE WHEN EXISTS (SELECT FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_name = '{_tableName}') "
                        + "THEN 1 ELSE 0 END AS Exists;";
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {dialectKind}.");
            }

            return query;
        }

        public static TableExistsBuilder Make()
        {
            return new TableExistsBuilder();
        }
    }
}

using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public sealed class ColumnExistsBuilder
    {
        private string _tableName;
        private string _columnName;

        public ColumnExistsBuilder Table(string tableName)
        {
            _tableName = tableName;
            return this;
        }

        public ColumnExistsBuilder Column(string columnName)
        {
            _columnName = columnName; 
            return this;
        }

        public IQuery Query(SqlDialectKinds dialectKind)
        {
            Validate();

            var query = new Query();

            _tableName = _tableName.ToLower();
            _columnName = _columnName.ToLower();

            switch (dialectKind)
            {
                case SqlDialectKinds.MsSql:
                    query.Text = $"IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'{_columnName}' AND Object_ID = Object_ID(N'{_tableName}')) SELECT 1 AS [Exists] ELSE SELECT 0 AS [Exists]";
                    break;
                case SqlDialectKinds.PgSql:
                    query.Text = $"SELECT EXISTS (SELECT 1 FROM information_schema.columns WHERE lower(table_name)='{_tableName}' AND lower(column_name)='{_columnName}');";
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {dialectKind}.");
            }

            return query;
        }

        public static ColumnExistsBuilder Make()
        {
            return new ColumnExistsBuilder();
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

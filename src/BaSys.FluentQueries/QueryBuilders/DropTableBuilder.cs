using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public sealed class DropTableBuilder
    {
        private string _tableName;

        public DropTableBuilder Table(string tableName)
        {
            _tableName = tableName;
            return this;
        }

        public IQuery Query(SqlDialectKinds dbKind)
        {
         
            IQuery query = new Query();

            switch (dbKind)
            {
                case SqlDialectKinds.MsSql:
                case SqlDialectKinds.PgSql:
                    query.Text = $"DROP TABLE IF EXISTS {_tableName};";
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {dbKind}.");

            }

            return query;

        }

        public static DropTableBuilder Make()
        {
            return new DropTableBuilder();
        }

      
    }
}

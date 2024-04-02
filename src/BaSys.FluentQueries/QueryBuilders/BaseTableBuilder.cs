using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.QueryBuilders
{
    public abstract class BaseTableBuilder<T> where T : new()
    {
        private string _tableName;
        private static T _builder;

        public string TableName => _tableName;

        public abstract IQuery GetMsSqlQuery();
        public abstract IQuery GetPgSqlQuery();

        public T Table(string tableName)
        {
            _tableName = tableName.ToLower();
            return _builder;
        }

        public virtual T Validate()
        {
            if (string.IsNullOrWhiteSpace(_tableName))
            {
                throw new ArgumentException($"{GetType().Name}. Table name is empty.");
            }

            return _builder;
        }

        public static T Make()
        {
            _builder = new T();
            return _builder;
        }

        public IQuery Query(SqlDialectKinds dialectKind)
        {
            IQuery query = null;

            switch (dialectKind)
            {
                case SqlDialectKinds.MsSql:
                    query = GetMsSqlQuery();
                    break;
                case SqlDialectKinds.PgSql:
                    query = GetPgSqlQuery();
                    break;
                default:
                    throw new NotImplementedException($"{GetType().Name} not implemented for DbKind {dialectKind}.");
            }

            return query;
        }
    }
}

﻿using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.QueryResults;
using Dapper;
using Npgsql;
using System.Data;

namespace BaSys.Host.DAL.Abstractions
{
    public abstract class TableManagerBase : ITableManager
    {
        protected readonly IDbConnection _connection;
        protected readonly SqlDialectKinds _sqlDialectKind;
        protected string _tableName;

        protected TableManagerBase(IDbConnection connection, string tableName)
        {
            _connection = connection;
            _sqlDialectKind = GetDialectKind(_connection);
            _tableName = tableName;
        }

        public string TableName => _tableName;

        public abstract Task<int> CreateTableAsync(IDbTransaction transaction = null);

        public virtual async Task<int> DropTableAsync(IDbTransaction transaction = null)
        {
            var query = DropTableBuilder.Make().Table(_tableName).Query(_sqlDialectKind);

            var result = await _connection.ExecuteAsync(query.Text, null, transaction);

            return result;
        }

        public virtual async Task<bool> TableExistsAsync(IDbTransaction transaction = null)
        {
            var query = TableExistsBuilder.Make().Table(_tableName).Query(_sqlDialectKind);

            var result = await _connection.QueryFirstOrDefaultAsync<TableExistsResult>(query.Text, null, transaction);

            return result.Exists;
        }

        private SqlDialectKinds GetDialectKind(IDbConnection connection)
        {
            var dialectKind = SqlDialectKinds.MsSql;
            if (connection is NpgsqlConnection)
            {
                dialectKind = SqlDialectKinds.PgSql;
            }

            return dialectKind;
        }
    }
}
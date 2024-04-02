using BaSys.FluentQueries.Enums;
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

        public virtual async Task<int> CreateTableAsync(IDbTransaction transaction = null)
        {
            var result = await CreateExtensionUuidOsspAsync(transaction);

            return result;
        }

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

        public async Task<bool> ColumnExistsAsync(string columnName, IDbTransaction transaction = null)
        {
            var query = ColumnExistsBuilder.Make().Table(_tableName).Column(columnName).Query(_sqlDialectKind);
            var result = await _connection.QueryFirstOrDefaultAsync<bool>(query.Text, null, transaction);

            return result;
        }

        public async Task<bool> ColumnExistsAsync_2(string columnName, IDbTransaction transaction = null)
        {
            var query = BaseTableBuilder<ColumnExistsBuilder_2>.Make().Table(_tableName).Column(columnName).Query(_sqlDialectKind);
            var result = await _connection.QueryFirstOrDefaultAsync<bool>(query.Text, null, transaction);

            return result;
        }

        public async Task<int> TruncateTableAsync(IDbTransaction transaction = null)
        {
            var query = TruncateTableBuilder.Make().Table(_tableName).Query(_sqlDialectKind);
            var result = await _connection.ExecuteAsync(query.Text, null, transaction);

            return result;
        }

        /// <summary>
        /// Creates extension uuid-ossp for GUID generating. Only for PG SQL.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<int> CreateExtensionUuidOsspAsync(IDbTransaction transaction)
        {
            if (_sqlDialectKind != SqlDialectKinds.PgSql)
            {
                return 0;
            }

            var result = await _connection.ExecuteAsync("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";", null, transaction);

            return result;
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

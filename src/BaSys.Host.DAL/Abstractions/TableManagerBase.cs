using BaSys.FluentQueries.Abstractions;
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
        protected IDataModelConfiguration _config;
        protected IQuery? _query;
        protected string _tableName;

        public IQuery? LastQuery => _query;

        protected TableManagerBase(IDbConnection connection, IDataModelConfiguration config)
        {
            _connection = connection;
            _sqlDialectKind = GetDialectKind(_connection);
            _config = config;
            _tableName = _config.TableName;
        }

        public string TableName =>  _tableName;

        public virtual async Task<int> CreateTableAsync(IDbTransaction? transaction = null)
        {
            await CreateExtensionUuidOsspAsync(transaction);

            _query = CreateTableBuilder.Make(_config).Table(_tableName).Query(_sqlDialectKind);

            var result = await _connection.ExecuteAsync(_query.Text, null, transaction);

            return result;
        }

        public virtual async Task<int> DropTableAsync(IDbTransaction? transaction = null)
        {
            _query = DropTableBuilder.Make().Table(_tableName).Query(_sqlDialectKind);

            var result = await _connection.ExecuteAsync(_query.Text, null, transaction);

            return result;
        }

        public virtual async Task<bool> TableExistsAsync(IDbTransaction? transaction = null)
        {
            _query = TableExistsBuilder.Make().Table(_tableName).Query(_sqlDialectKind);

            var result = await _connection.QueryFirstOrDefaultAsync<TableExistsResult>(_query.Text, null, transaction);

            return result.Exists;
        }

        public async Task<bool> ColumnExistsAsync(string columnName, IDbTransaction? transaction = null)
        {
            _query = ColumnExistsBuilder.Make().Table(_tableName).Column(columnName).Query(_sqlDialectKind);

            var result = await _connection.QueryFirstOrDefaultAsync<bool>(_query.Text, null, transaction);

            return result;
        }

        public async Task<int> TruncateTableAsync(IDbTransaction? transaction = null)
        {
            _query = TruncateTableBuilder.Make().Table(_tableName).Query(_sqlDialectKind);

            var result = await _connection.ExecuteAsync(_query.Text, null, transaction);

            return result;
        }

        /// <summary>
        /// Creates extension uuid-ossp for GUID generating. Only for PG SQL.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<int> CreateExtensionUuidOsspAsync(IDbTransaction? transaction)
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

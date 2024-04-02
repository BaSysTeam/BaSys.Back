using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Metadata.Models;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Abstractions
{
    public abstract class SystemObjectProviderBase<T> : ISystemObjectProvider<T> where T : class
    {
        protected readonly IDbConnection _dbConnection;
        protected SqlDialectKinds _sqlDialect;
        protected string _tableName;
        protected IQuery? _lastQuery;

        protected SystemObjectProviderBase(IDbConnection dbConnection, string tableName)
        {
            _dbConnection = dbConnection;
            _sqlDialect = GetDialectKind(dbConnection);
            _tableName = tableName;

            if (_tableName.Substring(0, 4) != "sys_")
                throw new ArgumentException($"Table name of system object have to start with prefix sys_");
        }

        public IQuery? LastQuery => _lastQuery;

        public virtual async Task<IEnumerable<T>> GetCollectionAsync(IDbTransaction transaction)
        {
            var query = SelectBuilder.Make().From(_tableName).Select("*").Query(_sqlDialect);

            _lastQuery = query;

            var result = await _dbConnection.QueryAsync<T>(query.Text, null, transaction);

            return result;
        }

        public virtual async Task<T> GetItemAsync(Guid uid, IDbTransaction transaction)
        {
            var query = SelectBuilder.Make()
              .From(_tableName)
              .Select("*")
              .WhereAnd("uid = @uid")
              .Parameter("uid", uid)
              .Query(_sqlDialect);

            _lastQuery = query;

            var result = await _dbConnection.QueryFirstOrDefaultAsync<T>(query.Text, query.DynamicParameters, transaction);

            return result;
        }

        public abstract Task<int> InsertAsync(T item, IDbTransaction transaction);

        public abstract Task<int> UpdateAsync(T item, IDbTransaction transaction);

        public virtual async Task<int> DeleteAsync(Guid uid, IDbTransaction transaction)
        {
            var query = DeleteBuilder.Make()
             .Table(_tableName)
             .WhereAnd("uid = @uid")
             .Parameter("uid", uid)
            .Query(_sqlDialect);
            _lastQuery = query;

            var result = await _dbConnection.ExecuteAsync(query.Text, query.DynamicParameters, transaction);

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

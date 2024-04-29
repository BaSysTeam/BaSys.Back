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
        protected IDataModelConfiguration _config;

        protected IQuery? _query;

        public IQuery? LastQuery => _query;


        protected SystemObjectProviderBase(IDbConnection dbConnection, IDataModelConfiguration config)
        {
            _dbConnection = dbConnection;
            _sqlDialect = GetDialectKind(dbConnection);
            _config = config;

            if (_config.TableName.Substring(0, 4) != "sys_")
                throw new ArgumentException($"Table name of system object have to start with prefix sys_");
            _config = config;
        }

        public virtual async Task<IEnumerable<T>> GetCollectionAsync(IDbTransaction transaction)
        {
            _query = SelectBuilder.Make().From(_config.TableName).Select("*").Query(_sqlDialect);

            var result = await _dbConnection.QueryAsync<T>(_query.Text, null, transaction);

            return result;
        }

        public virtual async Task<T> GetItemAsync(Guid uid, IDbTransaction transaction)
        {
            _query = SelectBuilder.Make()
              .From(_config.TableName)
              .Select("*")
              .WhereAnd("uid = @uid")
              .Parameter("uid", uid)
              .Query(_sqlDialect);

            var result = await _dbConnection.QueryFirstOrDefaultAsync<T>(_query.Text, _query.DynamicParameters, transaction);

            return result;
        }


        public abstract Task<int> InsertAsync(T item, IDbTransaction transaction);

        public abstract Task<int> UpdateAsync(T item, IDbTransaction transaction);

        public virtual async Task<int> DeleteAsync(Guid uid, IDbTransaction transaction)
        {
            _query = DeleteBuilder.Make()
            .Table(_config.TableName)
            .WhereAnd("uid = @uid")
            .Parameter("uid", uid)
           .Query(_sqlDialect);

            var result = await _dbConnection.ExecuteAsync(_query.Text, _query.DynamicParameters, transaction);

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

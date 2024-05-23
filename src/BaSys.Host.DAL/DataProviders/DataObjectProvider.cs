using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
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

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class DataObjectProvider : IDataObjectProvider
    {
        private readonly DataObjectConfiguration _config;
        private readonly IDbConnection _connection;
        private readonly SqlDialectKinds _sqlDialect;
        private readonly string _primaryKeyFieldName;

        protected IQuery? _query;

        public IQuery? LastQuery => _query;

        public DataObjectProvider(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            PrimitiveDataTypes primitiveDataTypes)
        {
            _connection = connection;

            _sqlDialect = GetDialectKind(connection);

            _config = new DataObjectConfiguration(kindSettings,
                objectSettings,
                primitiveDataTypes);

            var primaryKey = objectSettings.Header.PrimaryKey;
            _primaryKeyFieldName = primaryKey.Name;
        }

        public async Task<List<DataObject>> GetCollectionAsync(IDbTransaction? transaction)
        {
            _query = SelectBuilder.Make().From(_config.TableName).Select("*").Query(_sqlDialect);

            var dynamicCollection = await _connection.QueryAsync(_query.Text, null, transaction);

            var collection = new List<DataObject>();

            foreach (var dynamicItem in dynamicCollection)
            {
                var item = new DataObject((IDictionary<string, object>)dynamicItem);
                collection.Add(item);
            }

            return collection;
        }

        public async Task<DataObject?> GetItemAsync<T>(T uid, IDbTransaction? transaction)
        {
            _query = SelectBuilder.Make()
             .From(_config.TableName)
             .Select("*")
             .WhereAnd($"{_primaryKeyFieldName} = @{_primaryKeyFieldName}")
             .Parameter($"{_primaryKeyFieldName}", uid)
             .Query(_sqlDialect);

            var dynamicItem = await _connection.QueryFirstOrDefaultAsync(_query.Text, _query.DynamicParameters, transaction);

            if (dynamicItem != null)
            {
                var item = new DataObject((IDictionary<string, object>)dynamicItem);
                return item;
            }
            else
            {
                return null;
            }

        }

        public async Task<int> InsertAsync(DataObject item, IDbTransaction? transaction)
        {
            _query = InsertBuilder.Make(_config).FillValuesByColumnNames(true).Query(_sqlDialect);

            var result = await _connection.ExecuteAsync(_query.Text, item.Header, transaction);

            return result;
        }

        public Task<int> UpdateAsync(DataObject item, IDbTransaction? transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<int> DeleteAsync<T>(T uid, IDbTransaction? transaction)
        {
            _query = DeleteBuilder.Make()
                .Table(_config.TableName)
                .WhereAnd($"{_primaryKeyFieldName} = @{_primaryKeyFieldName}")
                .Parameter($"{_primaryKeyFieldName}", uid)
                .Query(_sqlDialect);

            var result = await _connection.ExecuteAsync(_query.Text, _query.DynamicParameters, transaction);

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

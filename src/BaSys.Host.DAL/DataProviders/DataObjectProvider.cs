using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Abstractions;
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
        private DbType _primaryKeyDbType;

        protected IQuery? _query;

        public IQuery? LastQuery => _query;

        public DataObjectProvider(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            IDataTypesIndex dataTypeIndex)
        {
            _connection = connection;

            _sqlDialect = GetDialectKind(connection);

            _config = new DataObjectConfiguration(kindSettings,
                objectSettings,
                dataTypeIndex);

            var primaryKey = objectSettings.Header.PrimaryKey;
            _primaryKeyFieldName = primaryKey.Name;

            var pkDataType = dataTypeIndex.GetDataTypeSafe(primaryKey.DataTypeUid);
            _primaryKeyDbType = pkDataType.DbType;


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

        public async Task<IEnumerable<SelectItem>> GetSelectItemsCollectionAsync(IDbTransaction? transaction)
        {

            _query = SelectBuilder.Make()
                .From(_config.TableName)
                .Select($"{_primaryKeyFieldName} as value")
                /// TODO: get presentation expressions instead of title
                .Select($"title as text")
                .Query(_sqlDialect);

            var collection = await _connection.QueryAsync<SelectItem>(_query.Text, null, transaction);

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

        public async Task<DataObject?> GetItemAsync(string uid, IDbTransaction? transaction)
        {
            DataObject? item = null;
            switch (_primaryKeyDbType)
            {
                case DbType.Int32:

                    int.TryParse(uid, out var intValue);
                    item = await GetItemAsync<int>(intValue, transaction);
                    break;

                case DbType.Int64:

                    long.TryParse(uid, out var longValue);
                    item = await GetItemAsync<long>(longValue, transaction);
                    break;

                case DbType.Guid:

                    Guid.TryParse(uid, out var guidValue);
                    item = await GetItemAsync<Guid>(guidValue, transaction);
                    break;

                case DbType.String:

                    item = await GetItemAsync<string>(uid, transaction);
                    break;

                default:
                    throw new ArgumentException($"Unsupported data type for primary key: {_primaryKeyDbType}");
            }

            return item;

        }

        public async Task<string> InsertAsync(DataObject item, IDbTransaction? transaction)
        {
            _query = InsertBuilder.Make(_config).PrimaryKeyName(_primaryKeyFieldName).ReturnId(true).FillValuesByColumnNames(true).Query(_sqlDialect);

            var result = await _connection.QueryFirstOrDefaultAsync<string>(_query.Text, item.Header, transaction);

            return result;
        }

        public async Task<int> UpdateAsync(DataObject item, IDbTransaction? transaction)
        {

            var uid = item.Header[_primaryKeyFieldName];

            _query = UpdateBuilder.Make(_config)
                   .WhereAnd($"{_primaryKeyFieldName} = @{_primaryKeyFieldName}")
                   .Parameter($"{_primaryKeyFieldName}", uid)
                   .Query(_sqlDialect);

            var result = await _connection.ExecuteAsync(_query.Text, item.Header, transaction);

            return result;
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

        public async Task<int> DeleteAsync(string uid, IDbTransaction? transaction)
        {
            var deletedCount = 0;
            switch (_primaryKeyDbType)
            {
                case DbType.Int32:

                    var intValue = int.Parse(uid);
                    deletedCount = await DeleteAsync<int>(intValue, transaction);
                    break;

                case DbType.Int64:

                    var longValue = long.Parse(uid);
                    deletedCount = await DeleteAsync<long>(longValue, transaction);
                    break;

                case DbType.Guid:

                    var guidValue = Guid.Parse(uid);
                    deletedCount = await DeleteAsync<Guid>(guidValue, transaction);
                    break;

                case DbType.String:

                    deletedCount = await DeleteAsync<string>(uid, transaction);
                    break;

                default:
                    throw new ArgumentException($"Unsupported data type for primary key: {_primaryKeyDbType}");
            }

            return deletedCount;
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

﻿using BaSys.DAL.Models.App;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Host.DAL.QueryResults;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using Dapper;
using System.Data;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class DataObjectProvider : DataObjectProviderBase, IDataObjectProvider
    {
        private readonly DataObjectConfiguration _config;

        public DataObjectProvider(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            IDataTypesIndex dataTypesIndex) : base(connection, kindSettings, objectSettings, dataTypesIndex)
        {

            _config = new DataObjectConfiguration(kindSettings,
                objectSettings,
                _dataTypesIndex);

        }

        public async Task<List<DataObject>> GetCollectionAsync(IDbTransaction? transaction)
        {
            var builder = SelectBuilder.Make().From(_config.TableName).Select("*");

            var orderByExpression = _objectSettings.GetOrderByExpression(_kindSettings.OrderByExpression);
            if (!string.IsNullOrWhiteSpace(orderByExpression))
            {
                builder.OrderBy(orderByExpression);
            }

            _query = builder.Query(_sqlDialect);

            var dynamicCollection = await _connection.QueryAsync(_query.Text, null, transaction);

            var collection = new List<DataObject>();

            foreach (var dynamicItem in dynamicCollection)
            {
                var item = new DataObject(_objectSettings, (IDictionary<string, object>)dynamicItem);
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
                var item = new DataObject(_objectSettings, (IDictionary<string, object>)dynamicItem);
                return item;
            }
            else
            {
                return null;
            }

        }

        public async Task<DataObject?> GetItemAsync(string uid, IDbTransaction? transaction)
        {

            DataObject? item = await ExecuteStronglyTypedAsync(uid, GetItemAsync, transaction);

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

        public async Task<int> UpdateFieldAsync(DataObject item, string columnName, object value, IDbTransaction? transaction)
        {
            var uid = item.Header[_primaryKeyFieldName];

            _query = UpdateBuilder.Make()
                .Table(_config.TableName)
                .Set(columnName)
                .Parameter(columnName, value)
                .WhereAnd($"{_primaryKeyFieldName} = @{_primaryKeyFieldName}")
                .Parameter($"{_primaryKeyFieldName}", uid)
                .Query(_sqlDialect);

            var result = await _connection.ExecuteAsync(_query.Text, _query.DynamicParameters, transaction);

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

            var deletedCount = await ExecuteStronglyTypedAsync(uid, DeleteAsync, transaction);

            return deletedCount;
        }

        public async Task<int> DeleteObjectRecordsAsync(string metaObjectColumnName,
            Guid metaObjectUid,
            string objectColumnName,
            object objectUid,
            IDbTransaction? transaction)
        {
            _query = DeleteBuilder.Make()
              .Table(_config.TableName)
              .WhereAnd($"{metaObjectColumnName} = @{metaObjectColumnName}")
              .Parameter($"{metaObjectColumnName}", metaObjectUid, DbType.Guid)
              .WhereAnd($"{objectColumnName} = @{objectColumnName}")
              .Parameter($"{objectColumnName}", objectUid)
              .Query(_sqlDialect);

            var result = await _connection.ExecuteAsync(_query.Text, _query.DynamicParameters, transaction);

            return result;
        }

        public async Task<long> CountAsync(IDbTransaction? transaction)
        {
            _query = SelectBuilder.Make().From(_config.TableName).Select("count(1) as ItemsCount").Query(_sqlDialect);

            var result = await _connection.QueryFirstOrDefaultAsync<ItemsCountResult>(_query.Text, null, transaction);

            return result.ItemsCount;
        }

    }
}

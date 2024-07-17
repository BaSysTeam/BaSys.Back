using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class DataObjectListProvider
    {
        private readonly DataObjectConfiguration _config;
        private readonly IDbConnection _connection;
        private readonly SqlDialectKinds _sqlDialect;
        private readonly MetaObjectKindSettings _kindSettings;
        private readonly MetaObjectStorableSettings _objectSettings;
        private readonly IEnumerable<MetaObjectKind> _allKinds;
        private readonly IEnumerable<MetaObjectStorable> _allMetaObjects;
        private readonly string _primaryKeyFieldName;
        private DbType _primaryKeyDbType;

        protected IQuery? _query;

        public IQuery? LastQuery => _query;

        public DataObjectListProvider(IDbConnection connection, 
            MetaObjectKindSettings kindSettings, 
            MetaObjectStorableSettings objectSettings, 
            IEnumerable<MetaObjectKind> allKinds,
            IEnumerable<MetaObjectStorable> allMetaObjects,
            IDataTypesIndex dataTypeIndex)
        {
            _connection = connection;

            _sqlDialect = GetDialectKind(connection);

            _config = new DataObjectConfiguration(kindSettings,
                objectSettings,
                dataTypeIndex);

            _kindSettings = kindSettings;
            _objectSettings = objectSettings;

            _allKinds = allKinds;
            _allMetaObjects = allMetaObjects;

            var primaryKey = objectSettings.Header.PrimaryKey;
            _primaryKeyFieldName = primaryKey.Name;

            var pkDataType = dataTypeIndex.GetDataTypeSafe(primaryKey.DataTypeUid);
            _primaryKeyDbType = pkDataType.DbType;

        }

        public async Task<List<DataObject>> GetCollectionWithDisplaysAsync(IDbTransaction? transaction)
        {
            var builder = SelectBuilder.Make().From(_config.TableName);

            foreach (var headerField in _objectSettings.Header.Columns)
            {
                var selectExpression = $"{_config.TableName}.{headerField.Name} AS {headerField.Name}";
                builder.Select(selectExpression);
            }

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
                var item = new DataObject((IDictionary<string, object>)dynamicItem);
                collection.Add(item);
            }

            return collection;
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

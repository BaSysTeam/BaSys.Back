using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Helpers;
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
        private readonly IDataTypesIndex _dataTypeIndex;
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

            _dataTypeIndex = dataTypeIndex;

            var primaryKey = objectSettings.Header.PrimaryKey;
            _primaryKeyFieldName = primaryKey.Name;

            var pkDataType = dataTypeIndex.GetDataTypeSafe(primaryKey.DataTypeUid);
            _primaryKeyDbType = pkDataType.DbType;

        }

        public async Task<List<DataObject>> GetCollectionWithDisplaysAsync(IDbTransaction? transaction)
        {

            var joins = new Dictionary<string, bool>();

            var builder = SelectBuilder.Make().From(_config.TableName);

            foreach (var headerField in _objectSettings.Header.Columns)
            {

                var dataType = _dataTypeIndex.GetDataTypeSafe(headerField.DataTypeUid);
                builder.Field(_config.TableName, headerField.Name, headerField.Name);

                if (!dataType.IsPrimitive)
                {
                    // Reference type. Get display field for reference type.
                    var currentKind = _allKinds.FirstOrDefault(x => x.Uid == dataType.ObjectKindUid);

                    if (currentKind == null)
                    {
                        continue;
                    }

                    var currentMetaObject = _allMetaObjects.FirstOrDefault(x => x.Uid == dataType.Uid);

                    if (currentMetaObject == null)
                    {
                        continue;
                    }

                    var currentKindSettings = currentKind.ToSettings();
                    var currentSettings = currentMetaObject.ToSettings();
                    var currentPkName = currentSettings.Header.PrimaryKey.Name;

                    var refTableName = DataObjectConfiguration.ComposeTableName(currentKind.Prefix, currentMetaObject.Name);
                    var diplayExpression = currentSettings.GetDisplayExpression(currentKindSettings.DisplayExpression, currentPkName);

                    if (diplayExpression.Contains("{{"))
                    {
                        var displayTemplateGenerator = new DisplayTemplateScriptGenerator(_sqlDialect);
                        var displayExpression = displayTemplateGenerator.Build(diplayExpression, refTableName, $"{headerField.Name}_display");

                        builder.Select(displayExpression);
                    }
                    else
                    {
                        builder.Field(refTableName, diplayExpression, $"{headerField.Name}_display");
                    }

                    if (!joins.ContainsKey(refTableName))
                    {
                        var condition = new ConditionModel()
                        {
                            LeftTable = _config.TableName,
                            LeftField = headerField.Name,
                            ComparisionOperator = ComparisionOperators.Equal,
                            RightTable = refTableName,
                            RightField = currentPkName
                        };

                        var conditions = new List<ConditionModel>
                        {
                            condition
                        };

                        builder.Join(JoinKinds.Left, refTableName, conditions);

                        joins.Add(refTableName, true);
                    }

                }

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

using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Helpers;
using BaSys.Metadata.Models;
using Dapper;
using System.Data;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class DataObjectListProvider : DataObjectProviderBase
    {
        private readonly DataObjectConfiguration _config;
        private readonly IEnumerable<MetaObjectKind> _allKinds;
        private readonly IEnumerable<MetaObjectStorable> _allMetaObjects;

        public DataObjectListProvider(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            IEnumerable<MetaObjectKind> allKinds,
            IEnumerable<MetaObjectStorable> allMetaObjects,
            IDataTypesIndex dataTypesIndex) : base(connection, kindSettings, objectSettings, dataTypesIndex)
        {
            _config = new DataObjectConfiguration(kindSettings,
                objectSettings,
                _dataTypesIndex);

            _allKinds = allKinds;
            _allMetaObjects = allMetaObjects;
        }

        public async Task<List<DataObject>> GetCollectionWithDisplaysAsync(IDbTransaction? transaction)
        {
            var builder = BuildQueryWithDisplays();

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

        public async Task<DataObject?> GetItemWithDisplaysAsync(string uid, IDbTransaction? transaction)
        {

            DataObject? item = await ExecuteStronglyTypedAsync(uid, GetItemWithDisplaysAsync, transaction);

            return item;

        }

        public async Task<DataObject?> GetItemWithDisplaysAsync<T>(T uid, IDbTransaction? transaction)
        {
            _query = BuildQueryWithDisplays()
                .WhereAnd($"{_config.TableName}.{_primaryKeyFieldName} = @{_primaryKeyFieldName}")
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

        private SelectBuilder BuildQueryWithDisplays()
        {
            var joins = new Dictionary<string, bool>();

            var builder = SelectBuilder.Make().From(_config.TableName);

            foreach (var column in _objectSettings.Header.Columns)
            {

                var dataType = _dataTypesIndex.GetDataTypeSafe(column.DataTypeUid);
                builder.Field(_config.TableName, column.Name, column.Name);

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
                        var displayExpression = displayTemplateGenerator.Build(diplayExpression, refTableName, $"{column.Name}_display");

                        builder.Select(displayExpression);
                    }
                    else
                    {
                        builder.Field(refTableName, diplayExpression, $"{column.Name}_display");
                    }

                    if (!joins.ContainsKey(refTableName))
                    {
                        var condition = new ConditionModel()
                        {
                            LeftTable = _config.TableName,
                            LeftField = column.Name,
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

            return builder;
        }
    }
}

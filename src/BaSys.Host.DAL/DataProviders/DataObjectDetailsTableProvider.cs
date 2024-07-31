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
    public sealed class DataObjectDetailsTableProvider
    {
        private readonly DataObjectDetailTableConfiguration _config;
        private readonly IDbConnection _connection;
        private readonly SqlDialectKinds _sqlDialect;
        private readonly MetaObjectKindSettings _kindSettings;
        private readonly MetaObjectStorableSettings _objectSettings;
        private readonly MetaObjectTable _tableSettings;
        private readonly IDataTypesIndex _dataTypesIndex;
        private readonly IEnumerable<MetaObjectKind> _allKinds;
        private readonly IEnumerable<MetaObjectStorable> _allMetaObjects;

        private readonly string _primaryKeyFieldName;
        private DbType _primaryKeyDbType;

        protected IQuery? _query;

        public IQuery? LastQuery => _query;

        public DataObjectDetailsTableProvider(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            MetaObjectTable tableSettings,
            IEnumerable<MetaObjectKind> allKinds,
            IEnumerable<MetaObjectStorable> allMetaObjects,
            IDataTypesIndex dataTypesIndex)
        {

            _connection = connection;

            _sqlDialect = GetDialectKind(connection);

            _kindSettings = kindSettings;
            _objectSettings = objectSettings;
            _tableSettings = tableSettings;

            _allKinds = allKinds;
            _allMetaObjects = allMetaObjects;

            _dataTypesIndex = dataTypesIndex;

            _config = new DataObjectDetailTableConfiguration(_kindSettings, _objectSettings, _tableSettings, _dataTypesIndex);

            var primaryKey = objectSettings.Header.PrimaryKey;
            _primaryKeyFieldName = primaryKey.Name;

            var pkDataType = _dataTypesIndex.GetDataTypeSafe(primaryKey.DataTypeUid);
            _primaryKeyDbType = pkDataType.DbType;
        }

        public async Task<DataObjectDetailsTable> GetTableAsync(IDbTransaction? transaction)
        {
            var builder = SelectBuilder.Make().From(_config.TableName).Select("*").OrderBy("row_number");

            _query = builder.Query(_sqlDialect);

            var dynamicCollection = await _connection.QueryAsync(_query.Text, null, transaction);

            var detailTable = new DataObjectDetailsTable()
            {
                Uid = _tableSettings.Uid,
                Name = _tableSettings.Name,
                Title = _tableSettings.Title,
            };

            foreach (var dynamicItem in dynamicCollection)
            {
                var row = new DataObjectDetailsTableRow((IDictionary<string, object>)dynamicItem);
                detailTable.Rows.Add(row);
            }

            return detailTable;

        }

        public async Task<DataObjectDetailsTable> GetTableWithDisplaysAsync(IDbTransaction? transaction)
        {

            var joins = new Dictionary<string, bool>();

            var builder = SelectBuilder.Make().From(_config.TableName);

            foreach(var column in _tableSettings.Columns)
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

            builder.OrderBy("row_number");

            _query = builder.Query(_sqlDialect);

            var dynamicCollection = await _connection.QueryAsync(_query.Text, null, transaction);

            var detailTable = new DataObjectDetailsTable()
            {
                Uid = _tableSettings.Uid,
                Name = _tableSettings.Name,
                Title = _tableSettings.Title,
            };

            foreach (var dynamicItem in dynamicCollection)
            {
                var row = new DataObjectDetailsTableRow((IDictionary<string, object>)dynamicItem);
                detailTable.Rows.Add(row);
            }

            return detailTable;

        }

        public async Task<int> UpdateAsync(string objectUid, DataObjectDetailsTable table, IDbTransaction? transaction)
        {

            await DeleteObjectRowsAsync(objectUid, transaction);

            _query = InsertBuilder.Make(_config).PrimaryKeyName(_primaryKeyFieldName).FillValuesByColumnNames(true).Query(_sqlDialect);

            var insertedCount = 0;

            foreach(var row in table.Rows)
            {
                var result = await _connection.ExecuteAsync(_query.Text, row.Fields, transaction);
                insertedCount += result;
            }

            return insertedCount;
        }

        public async Task<int> DeleteObjectRowsAsync<T>(T objectUid, IDbTransaction? transaction)
        {
            _query = DeleteBuilder.Make()
                .Table(_config.TableName)
                .WhereAnd($"object_uid = @objectUid")
                .Parameter($"objectUid", objectUid)
                .Query(_sqlDialect);

            var result = await _connection.ExecuteAsync(_query.Text, _query.DynamicParameters, transaction);

            return result;
        }

        public async Task<int> DeleteObjectRowsAsync(string uid, IDbTransaction? transaction)
        {
            var deletedCount = 0;
            switch (_primaryKeyDbType)
            {
                case DbType.Int32:

                    var intValue = int.Parse(uid);
                    deletedCount = await DeleteObjectRowsAsync<int>(intValue, transaction);
                    break;

                case DbType.Int64:

                    var longValue = long.Parse(uid);
                    deletedCount = await DeleteObjectRowsAsync<long>(longValue, transaction);
                    break;

                case DbType.Guid:

                    var guidValue = Guid.Parse(uid);
                    deletedCount = await DeleteObjectRowsAsync<Guid>(guidValue, transaction);
                    break;

                case DbType.String:

                    deletedCount = await DeleteObjectRowsAsync<string>(uid, transaction);
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

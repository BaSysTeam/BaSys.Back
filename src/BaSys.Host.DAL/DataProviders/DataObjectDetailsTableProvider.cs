using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.FluentQueries.ScriptGenerators;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.Helpers;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Helpers;
using BaSys.Metadata.Models;
using Dapper;
using System.Data;
using System.Diagnostics;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class DataObjectDetailsTableProvider : DataObjectProviderBase
    {
        private readonly DataObjectDetailTableConfiguration _config;
        private readonly MetaObjectTable _tableSettings;
        private readonly IEnumerable<MetaObjectKind> _allKinds;
        private readonly IEnumerable<MetaObjectStorable> _allMetaObjects;

        public DataObjectDetailsTableProvider(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            MetaObjectTable tableSettings,
            IEnumerable<MetaObjectKind> allKinds,
            IEnumerable<MetaObjectStorable> allMetaObjects,
            IDataTypesIndex dataTypesIndex) : base(connection, kindSettings, objectSettings, dataTypesIndex)
        {

            _tableSettings = tableSettings;

            _allKinds = allKinds;
            _allMetaObjects = allMetaObjects;

            _config = new DataObjectDetailTableConfiguration(_kindSettings, _objectSettings, _tableSettings, _dataTypesIndex);
        }

        public async Task<DataObjectDetailsTable> GetTableAsync(string objectUid, IDbTransaction? transaction)
        {

            DataObjectDetailsTable? table = await ExecuteStronglyTypedAsync<DataObjectDetailsTable?>(objectUid, GetTableAsync, transaction);

            if (table == null)
            {
                table = InitEmptyTable();
            }

            return table;

        }

        public async Task<DataObjectDetailsTable> GetTableAsync<T>(T objectUid, IDbTransaction? transaction)
        {
            var builder = SelectBuilder.Make().From(_config.TableName).Select("*");

            var condition = $"{ScriptGeneratorBase.WrapName(_config.TableName, _sqlDialect)}.{ScriptGeneratorBase.WrapName("object_uid", _sqlDialect)} = @object_uid";
            builder.WhereAnd(condition)
             .Parameter($"object_uid", objectUid);
            builder.OrderBy("row_number");

            _query = builder.Query(_sqlDialect);

            var dynamicCollection = await _connection.QueryAsync(_query.Text, _query.DynamicParameters, transaction);

            var detailTable = InitEmptyTable();

            foreach (var dynamicItem in dynamicCollection)
            {
                var row = new DataObjectDetailsTableRow((IDictionary<string, object>)dynamicItem);
                detailTable.Rows.Add(row);
            }

            return detailTable;

        }

        public async Task<DataObjectDetailsTable> GetTableWithDisplaysAsync(string objectUid, IDbTransaction? transaction)
        {

            DataObjectDetailsTable? table = await ExecuteStronglyTypedAsync<DataObjectDetailsTable>(objectUid, GetTableWithDisplaysAsync, transaction);

            if (table == null)
            {
                table = InitEmptyTable();
            }

            return table;

        }

        public async Task<DataObjectDetailsTable> GetTableWithDisplaysAsync<T>(T objectUid, IDbTransaction? transaction)
        {

            var joins = new Dictionary<string, bool>();

            var builder = SelectBuilder.Make().From(_config.TableName);

            foreach (var column in _tableSettings.Columns)
            {
                var dataType = _dataTypesIndex.GetDataTypeSafe(column.DataSettings.DataTypeUid);
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
                        var joinCondition = new ConditionModel()
                        {
                            LeftTable = _config.TableName,
                            LeftField = column.Name,
                            ComparisionOperator = ComparisionOperators.Equal,
                            RightTable = refTableName,
                            RightField = currentPkName
                        };

                        var conditions = new List<ConditionModel>
                        {
                            joinCondition
                        };

                        builder.Join(JoinKinds.Left, refTableName, conditions);

                        joins.Add(refTableName, true);
                    }

                }
            }

            var condition = $"{ScriptGeneratorBase.WrapName(_config.TableName, _sqlDialect)}.{ScriptGeneratorBase.WrapName("object_uid", _sqlDialect)} = @object_uid";
            builder.WhereAnd(condition)
             .Parameter($"object_uid", objectUid);
            builder.OrderBy("row_number");

            _query = builder.Query(_sqlDialect);

            var dynamicCollection = await _connection.QueryAsync(_query.Text, _query.DynamicParameters, transaction);

            var detailTable = InitEmptyTable();

            foreach (var dynamicItem in dynamicCollection)
            {
                var row = new DataObjectDetailsTableRow((IDictionary<string, object>)dynamicItem);
                detailTable.Rows.Add(row);
            }

            return detailTable;

        }

        public async Task<int> InsertAsync(string objectUid, DataObjectDetailsTable table, IDbTransaction? transaction)
        {

            var affectedRows = await ExecuteStronglyTypedAsync<int>(objectUid, table, InsertAsync, transaction);

            return affectedRows;
        }

        public async Task<int> InsertAsync<T>(T objectUid, DataObjectDetailsTable table, IDbTransaction? transaction)
        {
            var insertedCount = 0;

            if (_sqlDialect == SqlDialectKinds.PgSql)
            {
                var rowNumber = 1;
                foreach (var row in table.Rows)
                {
                    row.RowNumber = rowNumber;
                    row.ObjectUid = objectUid;

                    rowNumber++;
                    insertedCount += 1;
                }

                var bulkInsertHelper = new BulkInsertHelper(_connection, _sqlDialect, _tableSettings, _config.TableName, _dataTypesIndex);
                bulkInsertHelper.Execute(table);
            }
            else
            {
                // TODO: Delete after implement Bulk insert for MS SQL.
                _query = InsertBuilder.Make(_config).PrimaryKeyName(_primaryKeyFieldName).FillValuesByColumnNames(true).Query(_sqlDialect);

                 insertedCount = 0;

                var data = new List<Dictionary<string, object>>();
                var rowNumber = 1;
                foreach (var row in table.Rows)
                {
                    row.RowNumber = rowNumber;
                    row.ObjectUid = objectUid;

                    data.Add(row.Fields);
                    rowNumber++;

                }

                var result = await _connection.ExecuteAsync(_query.Text, data, transaction);
                insertedCount += result;
            }

            return insertedCount;
        }

        public async Task<int> UpdateAsync(string objectUid, DataObjectDetailsTable table, IDbTransaction? transaction)
        {

            await DeleteTableAsync(objectUid, transaction);
            var insertedCount = await InsertAsync(objectUid, table, transaction);

            return insertedCount;
        }

        public async Task<int> DeleteTableAsync<T>(T objectUid, IDbTransaction? transaction)
        {
            _query = DeleteBuilder.Make()
                .Table(_config.TableName)
                .WhereAnd($"object_uid = @objectUid")
                .Parameter($"objectUid", objectUid)
                .Query(_sqlDialect);

            var result = await _connection.ExecuteAsync(_query.Text, _query.DynamicParameters, transaction);

            return result;
        }

        public async Task<int> DeleteTableAsync(string uid, IDbTransaction? transaction)
        {
            var deletedCount = await ExecuteStronglyTypedAsync<int>(uid, DeleteTableAsync, transaction);

            return deletedCount;
        }

        private DataObjectDetailsTable InitEmptyTable()
        {
            var table = new DataObjectDetailsTable()
            {
                Name = _tableSettings.Name,
                Title = _tableSettings.Title,
                Uid = _tableSettings.Uid,
            };

            return table;
        }

    }
}

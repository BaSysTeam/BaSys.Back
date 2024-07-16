using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Helpers;
using BaSys.Metadata.Models;
using Dapper;
using Npgsql;
using System.Data;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class SelectItemsProvider : ISelectItemsProvider
    {
        private readonly DataObjectConfiguration _config;
        private readonly IDbConnection _connection;
        private readonly SqlDialectKinds _sqlDialect;
        private readonly string _primaryKeyFieldName;
        private readonly MetaObjectKindSettings _kindSettings;
        private readonly MetaObjectStorableSettings _objectSettings;

        private DbType _primaryKeyDbType;

        protected IQuery? _query;

        public IQuery? LastQuery => _query;

        public SelectItemsProvider(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            IDataTypesIndex dataTypeIndex)
        {
            _connection = connection;

            _sqlDialect = GetDialectKind(connection);

            _config = new DataObjectConfiguration(kindSettings,
                objectSettings,
                dataTypeIndex);

            _kindSettings = kindSettings;
            _objectSettings = objectSettings;


            var primaryKey = objectSettings.Header.PrimaryKey;
            _primaryKeyFieldName = primaryKey.Name;

            var pkDataType = dataTypeIndex.GetDataTypeSafe(primaryKey.DataTypeUid);
            _primaryKeyDbType = pkDataType.DbType;
        }

        public async Task<IEnumerable<SelectItem>> GetCollectionAsync(IDbTransaction? transaction)
        {

            var builder = SelectBuilder.Make()
             .From(_config.TableName)
             .Select($"{_primaryKeyFieldName} as value");


            var displayExpression = _objectSettings.GetDisplayExpression(_kindSettings.DisplayExpression, _primaryKeyFieldName);
            var dislpayBuilder = new DisplayTemplateScriptGenerator(_sqlDialect);
            var displaySqlExpression = dislpayBuilder.Build(displayExpression, null, "text");

            builder.Select(displaySqlExpression);


            _query = builder.Query(_sqlDialect);

            var collection = await _connection.QueryAsync<SelectItem>(_query.Text, null, transaction);

            return collection;
        }

        public async Task<SelectItem?> GetItemAsync<T>(T uid, IDbTransaction? transaction)
        {

            var builder = SelectBuilder.Make()
                .From(_config.TableName)
                .Select($"{_primaryKeyFieldName} as value");

            var displayExpression = _objectSettings.GetDisplayExpression(_kindSettings.DisplayExpression, _primaryKeyFieldName);
            var dislpayBuilder = new DisplayTemplateScriptGenerator(_sqlDialect);
            var displaySqlExpression = dislpayBuilder.Build(displayExpression, null, "text");

            builder.Select(displaySqlExpression);

            builder.WhereAnd($"{_primaryKeyFieldName} = @{_primaryKeyFieldName}")
            .Parameter($"{_primaryKeyFieldName}", uid);

            _query = builder.Query(_sqlDialect);

            var item = await _connection.QueryFirstOrDefaultAsync<SelectItem>(_query.Text, _query.DynamicParameters, transaction);

            return item;
        }

        public async Task<SelectItem?> GetItemAsync(string uid, IDbTransaction? transaction)
        {
            SelectItem? item = null;
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

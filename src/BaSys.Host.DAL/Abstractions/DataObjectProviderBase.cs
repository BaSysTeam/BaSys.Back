using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.Host.DAL.Helpers;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.Host.DAL.Abstractions
{
    public abstract class DataObjectProviderBase
    {
        protected IDbConnection _connection;
        protected SqlDialectKinds _sqlDialect;
        protected MetaObjectKindSettings _kindSettings;
        protected MetaObjectStorableSettings _objectSettings;
        protected IDataTypesIndex _dataTypesIndex;
        protected string _primaryKeyFieldName;
        protected DbType _primaryKeyDbType;

        protected IQuery? _query;

        public IQuery? LastQuery => _query;


        protected DataObjectProviderBase(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            IDataTypesIndex dataTypesIndex)
        {
            _connection = connection;

            _sqlDialect = SqlDialectKindHelper.GetDialectKind(connection);

            _kindSettings = kindSettings;
            _objectSettings = objectSettings;
            _dataTypesIndex = dataTypesIndex;

            var primaryKey = objectSettings.Header.PrimaryKey;
            _primaryKeyFieldName = primaryKey.Name;

            var pkDataType = _dataTypesIndex.GetDataTypeSafe(primaryKey.DataTypeUid);
            _primaryKeyDbType = pkDataType.DbType;
        }

        protected async Task<TResult?> ExecuteStronglyTypedAsync<TResult>(string objectUid,
         Func<object, IDbTransaction?, Task<TResult?>> func,
         IDbTransaction? transaction)
        {
            TResult? result = default;
            switch (_primaryKeyDbType)
            {
                case DbType.Int32:
                    if (int.TryParse(objectUid, out var intValue))
                        result = await func(intValue, transaction);
                    break;
                case DbType.Int64:
                    if (long.TryParse(objectUid, out var longValue))
                        result = await func(longValue, transaction);
                    break;
                case DbType.Guid:
                    if (Guid.TryParse(objectUid, out var guidValue))
                        result = await func(guidValue, transaction);
                    break;
                case DbType.String:
                    result = await func(objectUid, transaction);
                    break;
                default:
                    throw new ArgumentException($"Unsupported data type for primary key: {_primaryKeyDbType}");
            }
            return result;
        }

        protected async Task<TResult?> ExecuteStronglyTypedAsync<TResult>(string objectUid,
            DataObjectDetailsTable table,
           Func<object, DataObjectDetailsTable, IDbTransaction?, Task<TResult?>> func,
           IDbTransaction? transaction)
        {
            TResult? result = default;
            switch (_primaryKeyDbType)
            {
                case DbType.Int32:
                    if (int.TryParse(objectUid, out var intValue))
                        result = await func(intValue, table, transaction);
                    break;
                case DbType.Int64:
                    if (long.TryParse(objectUid, out var longValue))
                        result = await func(longValue, table, transaction);
                    break;
                case DbType.Guid:
                    if (Guid.TryParse(objectUid, out var guidValue))
                        result = await func(guidValue, table, transaction);
                    break;
                case DbType.String:
                    result = await func(objectUid, table, transaction);
                    break;
                default:
                    throw new ArgumentException($"Unsupported data type for primary key: {_primaryKeyDbType}");
            }
            return result;
        }

    }
}

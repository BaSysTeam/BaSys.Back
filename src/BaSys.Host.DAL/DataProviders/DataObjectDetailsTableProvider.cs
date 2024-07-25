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
    public sealed class DataObjectDetailsTableProvider
    {
        private readonly DataObjectDetailTableConfiguration _config;
        private readonly IDbConnection _connection;
        private readonly SqlDialectKinds _sqlDialect;
        private readonly MetaObjectKindSettings _kindSettings;
        private readonly MetaObjectStorableSettings _objectSettings;
        private readonly MetaObjectTable _tableSettings;
        private readonly string _primaryKeyFieldName;
        private DbType _primaryKeyDbType;

        protected IQuery? _query;

        public IQuery? LastQuery => _query;

        public DataObjectDetailsTableProvider(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            MetaObjectTable tableSettings,
            IDataTypesIndex dataTypeIndex)
        {

            _connection = connection;

            _sqlDialect = GetDialectKind(connection);

            _kindSettings = kindSettings;
            _objectSettings = objectSettings;
            _tableSettings = tableSettings;

            _config = new DataObjectDetailTableConfiguration(_kindSettings, _objectSettings, _tableSettings, dataTypeIndex);

            var primaryKey = objectSettings.Header.PrimaryKey;
            _primaryKeyFieldName = primaryKey.Name;

            var pkDataType = dataTypeIndex.GetDataTypeSafe(primaryKey.DataTypeUid);
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
            };

            foreach (var dynamicItem in dynamicCollection)
            {
                var row = new DataObjectDetailsTableRow((IDictionary<string, object>)dynamicItem);
                detailTable.Rows.Add(row);
            }

            return detailTable;

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

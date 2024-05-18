using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
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
    public sealed class DataObjectProvider: IDataObjectProvider
    {
        private readonly DataObjectConfiguration _config;
        private readonly IDbConnection _connection;
        private readonly SqlDialectKinds _sqlDialect;

        protected IQuery? _query;

        public IQuery? LastQuery => _query;

        public DataObjectProvider(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings, 
            PrimitiveDataTypes primitiveDataTypes)
        {
            _connection = connection;

            _sqlDialect = GetDialectKind(connection);

            _config = new DataObjectConfiguration(kindSettings,
                objectSettings,
                primitiveDataTypes);
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

        public Task<DataObject> GetItemAsync<T>(T uid, IDbTransaction? transaction)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAsync(DataObject item, IDbTransaction? transaction)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(DataObject item, IDbTransaction? transaction)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync<T>(T uid, IDbTransaction? transaction)
        {
            throw new NotImplementedException();
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

using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using Dapper;
using System.Data;

namespace BaSys.Host.DAL.TableManagers
{
    public sealed class DataObjectManager : TableManagerBase
    {
        public DataObjectManager(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            IDataTypesIndex dataTypeIndex) :

            base(connection, new DataObjectConfiguration(kindSettings,
                objectSettings,
                dataTypeIndex))
        {

        }

        public async Task<int> AlterTableAsync(AlterTableModel model, IDbTransaction? transaction = null)
        {
            var builder = new AlterTableBuilder(model).Table(_tableName);
            _query = builder.Query(_sqlDialectKind);

            var result = await _connection.ExecuteAsync(_query.Text, null, transaction);

            return result;
        }
    }
}

using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Helpers;
using BaSys.Metadata.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.TableManagers
{
    public sealed class DataObjectDetailTableManager : TableManagerBase
    {
        public DataObjectDetailTableManager(IDbConnection connection,
            MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            MetaObjectTable tableSettings,
            IDataTypesIndex dataTypeIndex) :

            base(connection,
                new DataObjectDetailTableConfiguration(kindSettings,
                    objectSettings,
                    tableSettings,
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

using System.Data;
using BaSys.FluentQueries.Abstractions;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using Dapper;

namespace BaSys.Host.DAL.TableManagers;

public class UserSettingsManager : TableManagerBase
{
    public UserSettingsManager(IDbConnection connection) : base(connection, new UserSettingsConfiguration())
    {
    }
    
    public override async Task<int> CreateTableAsync(IDbTransaction? transaction = null)
    {
        await base.CreateTableAsync(transaction);

        _query = CreateTableBuilder.Make(_config).Query(_sqlDialectKind);

        var result = await _connection.ExecuteAsync(_query.Text, null, transaction);

        return result;
    }
}
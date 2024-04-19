using System.Data;
using BaSys.DAL.Models.App;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using Dapper;

namespace BaSys.Host.DAL.DataProviders;

public class UserSettingsProvider : SystemObjectProviderBase<UserSettings>
{
    public UserSettingsProvider(IDbConnection dbConnection) : base(dbConnection, new UserSettingsConfiguration())
    {
    }

    public override async Task<int> InsertAsync(UserSettings item, IDbTransaction transaction)
    {
        _query = InsertBuilder.Make(_config)
            .FillValuesByColumnNames(true)
            .Query(_sqlDialect);

        var result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

        return result;
    }

    public override async Task<int> UpdateAsync(UserSettings item, IDbTransaction transaction)
    {
        _query = UpdateBuilder.Make(_config)
            .WhereAnd("uid = @uid")
            .Query(_sqlDialect);

        var result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

        return result;
    }
}
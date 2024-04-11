using System.Data;
using BaSys.DAL.Models;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using Dapper;

namespace BaSys.Host.DAL.DataProviders;

public class MigrationsProvider : SystemObjectProviderBase<Migration>
{
    public MigrationsProvider(IDbConnection dbConnection) : base(dbConnection, new MigrationsConfiguration())
    {
    }

    public override async Task<int> InsertAsync(Migration item, IDbTransaction transaction)
    {
        _query = InsertBuilder.Make(_config)
            .FillValuesByColumnNames(true)
            .Query(_sqlDialect);

        return await _dbConnection.ExecuteAsync(_query.Text, item, transaction);
    }
    
    public override async Task<int> UpdateAsync(Migration item, IDbTransaction transaction)
    {
        _query = UpdateBuilder.Make(_config)
            .WhereAnd("uid = @uid")
            .Query(_sqlDialect);

        return await _dbConnection.ExecuteAsync(_query.Text, item, transaction);
    }
}
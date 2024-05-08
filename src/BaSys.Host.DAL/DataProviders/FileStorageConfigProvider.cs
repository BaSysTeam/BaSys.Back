using System.Data;
using BaSys.DAL.Models.Admin;
using BaSys.FluentQueries.QueryBuilders;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using Dapper;

namespace BaSys.Host.DAL.DataProviders;

public class FileStorageConfigProvider : SystemObjectProviderBase<FileStorageConfig>
{
    public FileStorageConfigProvider(IDbConnection dbConnection) : base(dbConnection, new FileStorageConfigConfiguration())
    {
    }

    public override async Task<int> InsertAsync(FileStorageConfig item, IDbTransaction transaction)
    {
        _query = InsertBuilder.Make(_config)
            .FillValuesByColumnNames(true)
            .Query(_sqlDialect);

        return await _dbConnection.ExecuteAsync(_query.Text, item, transaction);
    }

    public override async Task<int> UpdateAsync(FileStorageConfig item, IDbTransaction transaction)
    {
        _query = UpdateBuilder.Make(_config)
            .WhereAnd("uid = @uid")
            .Query(_sqlDialect);

        var result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);
        return result;
    }
}
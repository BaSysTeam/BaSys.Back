using BaSys.DAL.Models.Admin;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using System.Data;

namespace BaSys.Host.DAL.DataProviders
{
    public sealed class AppConstantsProvider : SystemObjectProviderBase<AppConstants>
    {
        public AppConstantsProvider(IDbConnection dbConnection) : base(dbConnection, new AppConstantsConfiguration())
        {
        }

        //public override async Task<Guid> InsertAsync(AppConstants item, IDbTransaction transaction)
        //{

        //    if (item.Uid == Guid.Empty)
        //    {
        //        item.Uid = Guid.NewGuid();
        //    }

        //    _query = InsertBuilder.Make(_config)
        //        .FillValuesByColumnNames(true)
        //        .Query(_sqlDialect);

        //    var insertedCount = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

        //    var result = InsertedUid(insertedCount, item.Uid);

        //    return result;
        //}

        //public override async Task<int> UpdateAsync(AppConstants item, IDbTransaction transaction)
        //{

        //    _query = UpdateBuilder.Make(_config)
        //       .WhereAnd("uid = @uid")
        //       .Query(_sqlDialect);

        //    var result = await _dbConnection.ExecuteAsync(_query.Text, item, transaction);

        //    return result;
        //}
    }
}

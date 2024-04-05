using BaSys.Admin.Abstractions;
using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using BaSys.Common.Models;
using BaSys.Host.DAL;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.Identity.Models;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.SuperAdmin.DAL.Models;
using BaSys.SuperAdmin.DAL.Providers;
using BaSys.Translation;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Xml.Linq;

namespace BaSys.Admin.Services
{
    public sealed class AppConstantsRecordsService : IAppConstantsRecordsService
    {
        private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
        private readonly IBaSysConnectionFactory _baSysConnectionFactory;
        public AppConstantsRecordsService(
            IDbInfoRecordsProvider dbInfoRecordsProvider, 
            IBaSysConnectionFactory baSysConnectionFactory)
        {
            _dbInfoRecordsProvider = dbInfoRecordsProvider;
            _baSysConnectionFactory = baSysConnectionFactory;
        }

        public async Task<ResultWrapper<int>> CreateAppConstantsRecordAsync(AppConstantsRecordDto appConstant, string dbName)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var appConstantsRecord = new AppConstantsRecord(appConstant);
                    appConstantsRecord.Uid = Guid.NewGuid();
                    if (appConstantsRecord.DataBaseUid == Guid.Empty)
                    {
                        result.Error(-1, DictMain.WrongDataBaseUidFormat);
                        return result;
                    }
                    if (string.IsNullOrEmpty(appConstantsRecord.ApplicationTitle))
                    {
                        result.Error(-1, DictMain.EmptyApplicationTitle);
                        return result;
                    }

                    var provider = new AppConstantsRecordProvider(connection);
                    var collection = await provider.GetCollectionAsync(null);

                    if (!collection.Any())
                    {
                        var insertResult = await provider.InsertAsync(appConstantsRecord, null);
                        result.Success(insertResult, DictMain.AppConstantsRecordCreated);
                    }
                    else
                    {
                        result.Error(-1, DictMain.CannotCreateAppConstantsRecord);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotCreateAppConstantsRecord, ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteAppConstantsRecordAsync(Guid uid, string dbName)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var provider = new AppConstantsRecordProvider(connection);
                    var deletionResult = await provider.DeleteAsync(uid, null);

                    result.Success(deletionResult, DictMain.AppConstantsRecordDeleted);
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotDeleteAppConstantsRecord}: {uid}.", ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<AppConstantsRecordDto>> GetAppConstantsRecordAsync(string dbName)
        {
            var result = new ResultWrapper<AppConstantsRecordDto>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var provider = new AppConstantsRecordProvider(connection);
                    var collection = await provider.GetCollectionAsync(null);
                    var appConstantsRecord = collection.FirstOrDefault();
                    var dto = new AppConstantsRecordDto(appConstantsRecord);

                    result.Success(dto);
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotFindAppConstantsRecord, ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateAppConstantsRecordAsync(AppConstantsRecordDto appConstant, string dbName)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var appConstantsRecord = new AppConstantsRecord(appConstant);
                    if (appConstantsRecord.DataBaseUid == Guid.Empty)
                    {
                        result.Error(-1, DictMain.WrongDataBaseUidFormat);
                        return result;
                    }
                    if (string.IsNullOrEmpty(appConstantsRecord.ApplicationTitle))
                    {
                        result.Error(-1, DictMain.EmptyApplicationTitle);
                        return result;
                    }

                    var provider = new AppConstantsRecordProvider(connection);
                    var updateResult = await provider.UpdateAsync(appConstantsRecord, null);
                    result.Success(updateResult, DictMain.AppConstantsRecordUpdated);
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotUpdateAppConstantsRecord, ex.Message);
            }

            return result;
        }
    }
}

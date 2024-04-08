using BaSys.Admin.Abstractions;
using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.Admin;
using BaSys.DTO.Admin;
using BaSys.Host.DAL;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.Identity.Models;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.SuperAdmin.DAL.Models;
using BaSys.SuperAdmin.DAL.Providers;
using BaSys.Translation;
using Elfie.Serialization;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Xml.Linq;

namespace BaSys.Admin.Services
{
    public sealed class AppConstantsService : IAppConstantsService
    {
        private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
        private readonly IBaSysConnectionFactory _baSysConnectionFactory;
        public AppConstantsService(
            IDbInfoRecordsProvider dbInfoRecordsProvider, 
            IBaSysConnectionFactory baSysConnectionFactory)
        {
            _dbInfoRecordsProvider = dbInfoRecordsProvider;
            _baSysConnectionFactory = baSysConnectionFactory;
        }

        public async Task<ResultWrapper<int>> CreateAppConstantsAsync(AppConstantsDto dto, string dbName)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var appConstants = new AppConstants
                    {
                        Uid = Guid.NewGuid()
                    };

                    if (dto != null)
                    {
                        if (Guid.TryParse(dto.Uid, out var uid))
                            appConstants.Uid = uid;
                        if (Guid.TryParse(dto.DataBaseUid, out var dataBaseUid))
                            appConstants.DataBaseUid = dataBaseUid;

                        appConstants.ApplicationTitle = dto.ApplicationTitle;
                    }

                    if (appConstants.DataBaseUid == Guid.Empty)
                    {
                        result.Error(-1, DictMain.WrongDataBaseUidFormat);
                        return result;
                    }
                    if (string.IsNullOrEmpty(appConstants.ApplicationTitle))
                    {
                        result.Error(-1, DictMain.EmptyApplicationTitle);
                        return result;
                    }

                    var provider = new AppConstantsProvider(connection);
                    var collection = await provider.GetCollectionAsync(null);

                    if (!collection.Any())
                    {
                        var insertResult = await provider.InsertAsync(appConstants, null);
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

        public async Task<ResultWrapper<int>> DeleteAppConstantsAsync(Guid uid, string dbName)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var provider = new AppConstantsProvider(connection);
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

        public async Task<ResultWrapper<AppConstantsDto>> GetAppConstantsAsync(string dbName)
        {
            var result = new ResultWrapper<AppConstantsDto>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var provider = new AppConstantsProvider(connection);
                    var collection = await provider.GetCollectionAsync(null);
                    var appConstants = collection.FirstOrDefault();
                    if (appConstants == null)
                    {
                        result.Error(-1, DictMain.CannotFindAppConstantsRecord);
                        return result;
                    }

                    var dto = new AppConstantsDto
                    {
                        Uid = appConstants.Uid.ToString(),
                        DataBaseUid = appConstants.DataBaseUid.ToString(),
                        ApplicationTitle = appConstants.ApplicationTitle
                    };

                    dto.AppVersion = GetType()?.Assembly?.GetName()?.Version?.ToString() ?? string.Empty;

                    result.Success(dto);
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotFindAppConstantsRecord, ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateAppConstantsAsync(AppConstantsDto dto, string dbName)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var appConstants = new AppConstants();

                    if (dto != null)
                    {
                        if (Guid.TryParse(dto.Uid, out var uid))
                            appConstants.Uid = uid;
                        if (Guid.TryParse(dto.DataBaseUid, out var dataBaseUid))
                            appConstants.DataBaseUid = dataBaseUid;

                        appConstants.ApplicationTitle = dto.ApplicationTitle;
                    }

                    if (appConstants.DataBaseUid == Guid.Empty)
                    {
                        result.Error(-1, DictMain.WrongDataBaseUidFormat);
                        return result;
                    }
                    if (string.IsNullOrEmpty(appConstants.ApplicationTitle))
                    {
                        result.Error(-1, DictMain.EmptyApplicationTitle);
                        return result;
                    }

                    var provider = new AppConstantsProvider(connection);
                    var updateResult = await provider.UpdateAsync(appConstants, null);
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

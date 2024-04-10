using BaSys.Admin.Abstractions;
using BaSys.Admin.DTO;
using BaSys.Common;
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
using System.Reflection;
using System.Xml.Linq;

namespace BaSys.Admin.Services
{
    public sealed class AppConstantsService : IAppConstantsService
    {
        private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
        private readonly IBaSysConnectionFactory _baSysConnectionFactory;
        private readonly IHostVersionService _hostVersionService;
        public AppConstantsService(
            IDbInfoRecordsProvider dbInfoRecordsProvider, 
            IBaSysConnectionFactory baSysConnectionFactory,
            IHostVersionService hostVersionService)
        {
            _dbInfoRecordsProvider = dbInfoRecordsProvider;
            _baSysConnectionFactory = baSysConnectionFactory;
            _hostVersionService = hostVersionService;
        }

        public async Task<ResultWrapper<int>> CreateAppConstantsAsync(AppConstantsDto dto, string dbName)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var appConstants = dto.ToModel();
                    appConstants.Uid = Guid.NewGuid();

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

                    var dto = new AppConstantsDto(appConstants);
                    dto.AppVersion = _hostVersionService.GetVersion();

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
                    var appConstants = dto.ToModel();

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

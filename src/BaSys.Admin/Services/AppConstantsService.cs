using BaSys.Admin.Abstractions;
using BaSys.Common;
using BaSys.Common.Infrastructure;
using BaSys.DTO.Admin;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Translation;
using System.Data;

namespace BaSys.Admin.Services
{
    public sealed class AppConstantsService : IAppConstantsService, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly IHostVersionService _hostVersionService;
        private readonly AppConstantsProvider _provider;

        public AppConstantsService(
            IHostVersionService hostVersionService,
            IMainConnectionFactory mainConnectionFactory,
            ISystemObjectProviderFactory providerFactory)
        {
            _hostVersionService = hostVersionService;
            _connection = mainConnectionFactory.CreateConnection();
            providerFactory.SetUp(_connection);
            _provider = providerFactory.Create<AppConstantsProvider>();
        }

        public async Task<ResultWrapper<int>> CreateAppConstantsAsync(AppConstantsDto dto)
        {
            var result = new ResultWrapper<int>();

            try
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

                var collection = await _provider.GetCollectionAsync(null);
                if (!collection.Any())
                {
                    var insertResult = await _provider.InsertAsync(appConstants, null);
                    result.Success(insertResult, DictMain.AppConstantsRecordCreated);
                }
                else
                {
                    result.Error(-1, DictMain.CannotCreateAppConstantsRecord);
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotCreateAppConstantsRecord, ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteAppConstantsAsync(Guid uid)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var deletionResult = await _provider.DeleteAsync(uid, null);
                result.Success(deletionResult, DictMain.AppConstantsRecordDeleted);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotDeleteAppConstantsRecord}: {uid}.", ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<AppConstantsDto>> GetAppConstantsAsync()
        {
            var result = new ResultWrapper<AppConstantsDto>();

            try
            {
                var collection = await _provider.GetCollectionAsync(null);
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
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotFindAppConstantsRecord, ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateAppConstantsAsync(AppConstantsDto dto)
        {
            var result = new ResultWrapper<int>();

            try
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

                var updateResult = await _provider.UpdateAsync(appConstants, null);
                result.Success(updateResult, DictMain.AppConstantsRecordUpdated);
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotUpdateAppConstantsRecord, ex.Message);
            }

            return result;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}

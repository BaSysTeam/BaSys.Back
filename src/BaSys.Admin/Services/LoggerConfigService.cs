using BaSys.Admin.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.Admin;
using BaSys.DAL.Models.Logging;
using BaSys.DTO.Admin;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.Translation;
using System.Data;

namespace BaSys.Admin.Services
{
    public sealed class LoggerConfigService : ILoggerConfigService
    {
        private readonly IDbConnection _connection;
        private readonly LoggerConfigProvider _provider;

        public LoggerConfigService(
            IMainConnectionFactory mainConnectionFactory,
            ISystemObjectProviderFactory providerFactory)
        {
            _connection = mainConnectionFactory.CreateConnection();
            providerFactory.SetUp(_connection);
            _provider = providerFactory.Create<LoggerConfigProvider>();
        }

        public async Task<ResultWrapper<int>> CreateLoggerConfigAsync(LoggerConfig loggerConfig)
        {
            var result = new ResultWrapper<int>();

            try
            {
                if (string.IsNullOrEmpty(loggerConfig.ConnectionString))
                {
                    result.Error(-1, DictMain.EmptyConnectionString);
                    return result;
                }

                var collection = await _provider.GetCollectionAsync(null);
                if (!collection.Any())
                {
                    var insertResult = await _provider.InsertAsync(loggerConfig, null);
                    result.Success(insertResult, DictMain.LoggerConfigCreated);
                }
                else
                {
                    result.Error(-1, DictMain.CannotCreateLoggerConfig);
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotCreateLoggerConfig, ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteLoggerConfigAsync(Guid uid)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var deletionResult = await _provider.DeleteAsync(uid, null);
                result.Success(deletionResult, DictMain.LoggerConfigDeleted);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotDeleteLoggerConfig}: {uid}.", ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<LoggerConfig>> GetLoggerConfigAsync()
        {
            var result = new ResultWrapper<LoggerConfig>();
            
            try
            {
                var collection = await _provider.GetCollectionAsync(null);
                var loggerConfig = collection.FirstOrDefault();
                if (loggerConfig == null)
                {
                    result.Error(-1, DictMain.CannotFindLoggerConfig);
                    return result;
                }

                result.Success(loggerConfig);
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotFindLoggerConfig, ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateLoggerConfigAsync(LoggerConfig loggerConfig)
        {
            var result = new ResultWrapper<int>();

            try
            {
                loggerConfig.ConnectionString = loggerConfig.ConnectionString?.Replace("\n", "");
                if (loggerConfig.IsEnabled)
                {
                    if (string.IsNullOrEmpty(loggerConfig.ConnectionString))
                    {
                        result.Error(-1, DictMain.EmptyConnectionString);
                        return result;
                    }
                    if (loggerConfig.LoggerType == null)
                    {
                        result.Error(-1, DictMain.EmptyLoggerType);
                        return result;
                    }
                }

                var updateResult = await _provider.UpdateAsync(loggerConfig, null);
                result.Success(updateResult, DictMain.LoggerConfigUpdated);
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotUpdateLoggerConfig, ex.Message);
            }

            return result;
        }
    }
}

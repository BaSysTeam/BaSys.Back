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
        private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
        private readonly IBaSysConnectionFactory _baSysConnectionFactory;

        public LoggerConfigService(
            IDbInfoRecordsProvider dbInfoRecordsProvider,
            IBaSysConnectionFactory baSysConnectionFactory)
        {
            _dbInfoRecordsProvider = dbInfoRecordsProvider;
            _baSysConnectionFactory = baSysConnectionFactory;
        }

        public async Task<ResultWrapper<int>> CreateLoggerConfigAsync(LoggerConfigDto dto, string dbName)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var loggerConfig = new LoggerConfig
                    {
                        Uid = Guid.NewGuid(),
                        ConnectionString = dto.ConnectionString,
                        MinimumLogLevel = dto.MinimumLogLevel,
                        LoggerType = dto.LoggerType,
                        IsEnabled = dto.IsEnabled,
                        AutoClearInterval = dto.AutoClearInterval
                    };

                    if (string.IsNullOrEmpty(loggerConfig.ConnectionString))
                    {
                        result.Error(-1, DictMain.EmptyConnectionString);
                        return result;
                    }

                    var provider = new LoggerConfigProvider(connection);
                    var collection = await provider.GetCollectionAsync(null);

                    if (!collection.Any())
                    {
                        var insertResult = await provider.InsertAsync(loggerConfig, null);
                        result.Success(insertResult, DictMain.LoggerConfigCreated);
                    }
                    else
                    {
                        result.Error(-1, DictMain.CannotCreateLoggerConfig);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotCreateLoggerConfig, ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteLoggerConfigAsync(Guid uid, string dbName)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var provider = new LoggerConfigProvider(connection);
                    var deletionResult = await provider.DeleteAsync(uid, null);

                    result.Success(deletionResult, DictMain.LoggerConfigDeleted);
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotDeleteLoggerConfig}: {uid}.", ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<LoggerConfigDto>> GetLoggerConfigAsync(string dbName)
        {
            var result = new ResultWrapper<LoggerConfigDto>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var provider = new LoggerConfigProvider(connection);
                    var collection = await provider.GetCollectionAsync(null);
                    var loggerConfig = collection.FirstOrDefault();
                    if (loggerConfig == null)
                    {
                        result.Error(-1, DictMain.CannotFindLoggerConfig);
                        return result;
                    }

                    var dto = new LoggerConfigDto
                    {
                        Uid = loggerConfig.Uid.ToString(),
                        AutoClearInterval = loggerConfig.AutoClearInterval,
                        ConnectionString = loggerConfig.ConnectionString,
                        IsEnabled = loggerConfig.IsEnabled,
                        LoggerType = loggerConfig.LoggerType,
                        MinimumLogLevel = loggerConfig.MinimumLogLevel
                    };

                    result.Success(dto);
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotFindLoggerConfig, ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateLoggerConfigAsync(LoggerConfigDto dto, string dbName)
        {
            var result = new ResultWrapper<int>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            try
            {
                using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
                {
                    var loggerConfig = new LoggerConfig
                    {
                        ConnectionString = dto.ConnectionString,
                        MinimumLogLevel = dto.MinimumLogLevel,
                        LoggerType = dto.LoggerType,
                        IsEnabled = dto.IsEnabled,
                        AutoClearInterval = dto.AutoClearInterval
                    };

                    if (Guid.TryParse(dto.Uid, out var uid))
                        loggerConfig.Uid = uid;

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

                    var provider = new LoggerConfigProvider(connection);
                    var updateResult = await provider.UpdateAsync(loggerConfig, null);

                    result.Success(updateResult, DictMain.LoggerConfigUpdated);
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotUpdateLoggerConfig, ex.Message);
            }

            return result;
        }
    }
}

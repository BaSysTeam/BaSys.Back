using BaSys.Common.Enums;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.SuperAdmin.DAL.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace BaSys.Logging.Services;

public class LoggerConfigService : ILoggerConfigService
{
    private readonly IBaSysConnectionFactory _connectionFactory;
    private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggerConfigService(IBaSysConnectionFactory connectionFactory,
        IDbInfoRecordsProvider dbInfoRecordsProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _dbInfoRecordsProvider = dbInfoRecordsProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoggerConfig> GetLoggerConfigAsync()
    {
        var dbNameClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "DbName");
        if (dbNameClaim == null)
            return new LoggerConfig();
        
        var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbNameClaim.Value);
        if (dbInfoRecord == null)
            return new LoggerConfig();

        LoggerConfig config;
        using (var connection = _connectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
        {
            config = await GetLoggerConfigAsync(connection, null);
        };

        return config;
    }

    public async Task<LoggerConfig> GetLoggerConfigAsync(IDbConnection connection, IDbTransaction? transaction)
    {
        // Get db uid
        var appConstantsProvider = new AppConstantsProvider(connection);
        var appConstants = (await appConstantsProvider.GetCollectionAsync(null))?.FirstOrDefault();
        if (appConstants == null)
            return new LoggerConfig();

        // Get logger connections
        var provider = new LoggerConfigProvider(connection);
        var config = (await provider.GetCollectionAsync(null))?.FirstOrDefault(x => x.IsSelected);

        if (config == null)
            return new LoggerConfig();

        return new LoggerConfig
        {
            LoggerType = config.LoggerType,
            MinimumLogLevel = config.MinimumLogLevel,
            ConnectionString = config.ConnectionString,
            IsEnabled = config.IsEnabled,
            AutoClearInterval = config.AutoClearInterval,
            DbUid = appConstants.DataBaseUid,
            TableName = LoggerConfig.GetTableName(appConstants.DataBaseUid),
            WorkflowsLogTableName = LoggerConfig.GetWorkflowsLogTableName(appConstants.DataBaseUid),
        };
    }

}
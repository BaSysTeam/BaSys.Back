using BaSys.Common.Enums;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.SuperAdmin.DAL.Abstractions;
using Microsoft.AspNetCore.Http;

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

    public async Task<LoggerConfig> GetLoggerConfig()
    {
        var dbNameClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "DbName");
        if (dbNameClaim == null)
            return new LoggerConfig();
        
        var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbNameClaim.Value);
        if (dbInfoRecord == null)
            return new LoggerConfig();
        
        using var connection = _connectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind);
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
            TableName = GetTableName(appConstants.DataBaseUid)
        };

        #region hardcode for test
        // // Mongo
        // return new LoggerConfig
        // {
        //     LoggerType = LoggerTypes.MongoDb,
        //     MinimumLogLevel = EventTypeLevels.Info,
        //     ConnectionString = "mongodb://localhost:27017/Serilog",
        //     DbUid = dbUid,
        //     TableName = tableName,
        //     IsEnabled = true,
        //     AutoClearInterval = AutoClearInterval.Month
        // };

        // // MsSql
        // return new LoggerConfig
        // {
        //     LoggerType = LoggerTypes.MsSql,
        //     MinimumLogLevel = EventTypeLevels.Info,
        //     ConnectionString =
        //         "Data Source=OSPC\\SQLEXPRESS19;Initial Catalog=__Serilog;Persist Security Info=True;User ID=sa;Password=QAZwsx!@#;TrustServerCertificate=True;",
        //     DbUid = dbUid,
        //     TableName = tableName,
        //     IsEnabled = true,
        //     AutoClearInterval = AutoClearInterval.Month
        // };

        // // PgSql
        // return new LoggerConfig
        // {
        //     LoggerType = LoggerTypes.PgSql,
        //     MinimumLogLevel = EventTypeLevels.Info,
        //     ConnectionString = "Server=localhost;Port=5432;Database=serilog;User ID=postgres;Password=QAZwsx!@#;Timeout=60;",
        //     DbUid = dbUid,
        //     TableName = tableName,
        //     IsEnabled = true,
        //     AutoClearInterval = AutoClearInterval.Month
        // };
        #endregion
    }

    private string GetTableName(Guid dbUid) => $"logs-{dbUid}";
}
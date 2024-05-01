using BaSys.Common.Enums;
using BaSys.Logging.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;

namespace BaSys.Logging.LogServices;

public class MongoLoggerService : LoggerService
{
    private readonly IMongoDatabase? _db;
    
    public MongoLoggerService(LoggerConfig loggerConfig, string? userUid, string? userName, string? ipAddress) 
        : base(loggerConfig, userUid, userName, ipAddress)
    {
        if (string.IsNullOrEmpty(loggerConfig.ConnectionString) || string.IsNullOrEmpty(loggerConfig.TableName))
            return;
        
        var client = new MongoClient(loggerConfig.ConnectionString);
        _db = client.GetDatabase(loggerConfig.TableName);

        if (IsMongoLive())
        {
            _logger = new LoggerConfiguration()
                .WriteTo
                .MongoDBBson(cfg =>
                {
                    cfg.SetMongoUrl(loggerConfig.ConnectionString);
                    cfg.SetCollectionName(loggerConfig.TableName);
                    cfg.SetExpireTTL(GetExpireTTL(loggerConfig));
                })
                .CreateLogger();
        }
    }

    protected override void WriteInner(string message,
        EventTypeLevels level,
        EventType eventType,
        Guid? metadataUid = null,
        string? dataUid = null,
        string? dataPresentation = null)
    {
        if (!IsMongoLive())
            return;
        
        _logger?.Information("{message} {Level} {EventTypeName} {EventTypeUid} {Module} {UserUid} {UserName} {IpAddress} {MetadataUid} {DataUid} {DataPresentation}",
            message,
            (int) level,
            eventType.EventName,
            eventType.Uid,
            eventType.Module,
            _userUid,
            _userName,
            _ipAddress,
            metadataUid,
            dataUid,
            dataPresentation);
    }
    
    private bool IsMongoLive()
    {
        var isMongoLive = _db.RunCommandAsync((Command<BsonDocument>) "{ping:1}").Wait(1000);
        return isMongoLive;
    }

    private TimeSpan? GetExpireTTL(LoggerConfig loggerConfig)
    {
        switch (loggerConfig.AutoClearInterval)
        {
            case AutoClearInterval.Week:
                return TimeSpan.FromDays(7);
            case AutoClearInterval.Month:
                return TimeSpan.FromDays(30);
            case AutoClearInterval.Quarter:
                return TimeSpan.FromDays(90);
            case AutoClearInterval.Year:
                return TimeSpan.FromDays(365);
            case AutoClearInterval.None:
            default:
                return null;
        }
    }
}
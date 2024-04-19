using System.Security.Claims;
using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DTO;
using BaSys.SuperAdmin.DAL.Abstractions;

namespace BaSys.Host.Services;

public class UserSettingsService : IUserSettingsService
{
    private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
    private readonly IBaSysConnectionFactory _connectionFactory;
    private readonly string? _dbName;
    private readonly string? _userId;

    public UserSettingsService(IDbInfoRecordsProvider dbInfoRecordsProvider,
        IBaSysConnectionFactory connectionFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbInfoRecordsProvider = dbInfoRecordsProvider;
        _connectionFactory = connectionFactory;
        _dbName = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "DbName")?.Value;
        _userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    public async Task<ResultWrapper<UserSettingsDto?>> GetUserSettings()
    {
        var result = new ResultWrapper<UserSettingsDto?>();
        if (string.IsNullOrEmpty(_dbName))
        {
            result.Error(-1, $"DbName is not set!");
            return result;
        }
        
        if (string.IsNullOrEmpty(_userId))
        {
            result.Error(-1, $"UserId is not set!");
            return result;
        }

        var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(_dbName);
        using var connection = _connectionFactory.CreateConnection(dbInfoRecord!.ConnectionString, dbInfoRecord.DbKind);
        var provider = new UserSettingsProvider(connection);

        var userSettings = await provider.GetItemByUserIdAsync(_userId);
        if (userSettings != null)
            result.Success(new UserSettingsDto(userSettings));
        else
            result.Success(new UserSettingsDto());

        return result;
    }

    public async Task<ResultWrapper<bool>> UpdateUserSettings(UserSettingsDto userSettings)
    {
        var result = new ResultWrapper<bool>();
        if (string.IsNullOrEmpty(_dbName))
        {
            result.Error(-1, $"DbName is not set!");
            return result;
        }
        
        var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(_dbName);
        using var connection = _connectionFactory.CreateConnection(dbInfoRecord!.ConnectionString, dbInfoRecord.DbKind);
        var provider = new UserSettingsProvider(connection);

        var state = await provider.UpdateAsync(userSettings.ToModel(), null);
        if (state == 1)
            result.Success(true);
        else
            result.Error(-1, "UserSettings update error");
        
        return result;
    }
}
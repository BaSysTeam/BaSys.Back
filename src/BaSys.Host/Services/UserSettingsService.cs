using System.Security.Claims;
using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DTO;

namespace BaSys.Host.Services;

public class UserSettingsService : IUserSettingsService
{
    private readonly IMainConnectionFactory _connectionFactory;
    private readonly string? _userId;

    public UserSettingsService(IHttpContextAccessor httpContextAccessor,
        IMainConnectionFactory connectionFactory)
    {
        _userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        _connectionFactory = connectionFactory;
    }

    public async Task<ResultWrapper<UserSettingsDto?>> GetUserSettings()
    {
        var result = new ResultWrapper<UserSettingsDto?>();
       
        if (string.IsNullOrEmpty(_userId))
        {
            result.Error(-1, $"UserId is not set!");
            return result;
        }

        using var connection = _connectionFactory.CreateConnection();
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
        
        using var connection = _connectionFactory.CreateConnection();
        var provider = new UserSettingsProvider(connection);

        var state = await provider.UpdateAsync(userSettings.ToModel(), null);
        if (state == 1)
            result.Success(true);
        else
            result.Error(-1, "UserSettings update error");
        
        return result;
    }
}
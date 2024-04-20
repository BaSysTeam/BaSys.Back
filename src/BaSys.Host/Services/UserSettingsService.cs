using System.Security.Claims;
using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DTO;
using BaSys.Host.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.Services;

public class UserSettingsService : IUserSettingsService
{
    private readonly IMainConnectionFactory _connectionFactory;
    private readonly UserManager<WorkDbUser> _userManager;
    private readonly string? _userId;

    public UserSettingsService(IHttpContextAccessor httpContextAccessor,
        UserManager<WorkDbUser> userManager,
        IMainConnectionFactory connectionFactory)
    {
        _userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        _connectionFactory = connectionFactory;
        _userManager = userManager;
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

        UserSettingsDto userSettingsDto;
        var userSettings = await provider.GetItemByUserIdAsync(_userId);
        if (userSettings != null)
            userSettingsDto = new UserSettingsDto(userSettings);
        else
            userSettingsDto = new UserSettingsDto();

        var currentUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == _userId);
        userSettingsDto.UserName = currentUser?.UserName ?? string.Empty;
        
        result.Success(userSettingsDto);
        return result;
    }

    public async Task<ResultWrapper<bool>> UpdateUserSettings(UserSettingsDto userSettings)
    {
        var result = new ResultWrapper<bool>();
        
        using var connection = _connectionFactory.CreateConnection();
        var provider = new UserSettingsProvider(connection);

        var currentUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == _userId);
        if (currentUser == null)
        {
            result.Error(-1, "User not found");
            return result;
        }

        currentUser.UserName = userSettings.UserName;
        await _userManager.UpdateAsync(currentUser);
        
        var state = await provider.UpdateAsync(userSettings.ToModel(), null);
        if (state == 1)
            result.Success(true);
        else
            result.Error(-1, "UserSettings update error");
        
        return result;
    }
}
using System.Globalization;
using System.Security.Claims;
using System.Text;
using BaSys.Common.DTO;
using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DTO;
using BaSys.Host.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.Services;

public class UserSettingsService : IUserSettingsService
{
    private readonly IMainConnectionFactory _connectionFactory;
    private readonly UserManager<WorkDbUser> _userManager;
    private readonly string? _userId;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserSettingsService(IHttpContextAccessor httpContextAccessor,
        UserManager<WorkDbUser> userManager,
        IMainConnectionFactory connectionFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        _connectionFactory = connectionFactory;
        _userManager = userManager;
    }

    public async Task<ResultWrapper<UserSettingsDto?>> GetUserSettings()
    {
        return await GetUserSettings(_userId);
    }

    public async Task<ResultWrapper<UserSettingsDto?>> GetUserSettings(string? userId)
    {
        var result = new ResultWrapper<UserSettingsDto?>();

        if (string.IsNullOrEmpty(userId))
        {
            result.Error(-1, $"UserId is not set!");
            return result;
        }

        using var connection = _connectionFactory.CreateConnection();
        var provider = new UserSettingsProvider(connection);

        var currentUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

        UserSettingsDto userSettingsDto;
        var userSettings = await provider.GetItemByUserIdAsync(userId);
        if (userSettings != null)
            userSettingsDto = new UserSettingsDto(userSettings);
        else
        {
            userSettingsDto = new UserSettingsDto
            {
                UserId = currentUser?.Id ?? string.Empty
            };

            var insertResult = await provider.InsertAsync(userSettingsDto.ToModel(), null);
            userSettings = await provider.GetItemByUserIdAsync(userId);
            if (insertResult < 1 || userSettings == null)
            {
                result.Error(-1, $"Cannot get user settings!");
                return result;
            }
            else
                userSettingsDto = new UserSettingsDto(userSettings);
        }

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
        
        var userSettingsDB = await provider.GetItemByUserIdAsync(_userId);
        var state = await provider.UpdateAsync(userSettings.ToModel(), null);
        if (state == 1)
        {
            if (userSettingsDB != null && 
                userSettingsDB.Language != userSettings.Language)
            {
                SetLocalization(userSettings.Language);
            }

            result.Success(true);
        }
        else
            result.Error(-1, "UserSettings update error");

        return result;
    }

    public ResultWrapper<List<EnumValuesDto>> GetLanguages()
    {
        var result = new ResultWrapper<List<EnumValuesDto>>();
        var languages = new List<EnumValuesDto>();
        foreach (var lang in (Languages[]) Enum.GetValues(typeof(Languages)))
        {
            languages.Add(new EnumValuesDto
            {
                Id = (int)lang,
                Name = lang.ToString()
            });
        }
        
        result.Success(languages);
        return result;
    }

    public async Task<ResultWrapper<bool>> ChangePassword(string? userId, string? oldPassword, string? newPassword)
    {
        var result = new ResultWrapper<bool>();
        if (string.IsNullOrEmpty(userId))
        {
            result.Error(-1, "UserId is empty!");
            return result;
        }
        
        if (string.IsNullOrEmpty(oldPassword))
        {
            result.Error(-1, "Old password is empty!");
            return result;
        }
        
        if (string.IsNullOrEmpty(newPassword))
        {
            result.Error(-1, "New password is empty!");
            return result;
        }

        var currentUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (currentUser == null)
        {
            result.Error(-1, "User not found!");
            return result;
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);
        if (changePasswordResult.Succeeded)
        {
            result.Success(true);
            return result;
        }
        else
        {
            var sb = new StringBuilder();
            foreach (var error in changePasswordResult.Errors)
            {
                sb.Append($"{error.Description}; ");
            }
            result.Error(-1, sb.ToString());
            
            return result;
        }
    }

    private void SetLocalization(Languages userLanguage)
    {
        var cultureName = userLanguage == Languages.English ? "en-US" : "ru-RU";
        var culture = CultureInfo.GetCultureInfo(cultureName);

        var defaultCookieName = CookieRequestCultureProvider.DefaultCookieName;
        var requestCulture = new RequestCulture(culture);
        var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

        _httpContextAccessor?.HttpContext?.Response.Cookies.Append(defaultCookieName, cookieValue);
    }
}
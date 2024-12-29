using BaSys.Common.DTO;
using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;
using BaSys.Host.DTO;
using System.Data;

namespace BaSys.Host.Abstractions;

public interface IUserSettingsService
{
    void SetUp(IDbConnection connection);
    Task<ResultWrapper<UserSettingsDto?>> GetUserSettings();
    Task<ResultWrapper<UserSettingsDto?>> GetUserSettings(string? userId);
    Task<ResultWrapper<bool>> UpdateUserSettings(UserSettingsDto userSettings);
    ResultWrapper<List<EnumValuesDto>> GetLanguages();
    Task<ResultWrapper<bool>> ChangePassword(string? userId, string? oldPassword, string? newPassword);
    Task<ResultWrapper<bool>> SetLanguageAsync(string userName, string language);
    
}
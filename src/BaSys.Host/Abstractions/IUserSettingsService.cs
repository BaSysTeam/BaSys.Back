using BaSys.Common.DTO;
using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;
using BaSys.Host.DTO;

namespace BaSys.Host.Abstractions;

public interface IUserSettingsService
{
    Task<ResultWrapper<UserSettingsDto?>> GetUserSettings();
    Task<ResultWrapper<UserSettingsDto?>> GetUserSettings(string? userId);
    Task<ResultWrapper<bool>> UpdateUserSettings(UserSettingsDto userSettings);
    ResultWrapper<List<EnumValuesDto>> GetLanguages();
    Task<ResultWrapper<bool>> ChangePassword(string? userId, string? oldPassword, string? newPassword);
}
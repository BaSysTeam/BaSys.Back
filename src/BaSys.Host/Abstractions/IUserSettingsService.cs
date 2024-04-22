using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;
using BaSys.Host.DTO;

namespace BaSys.Host.Abstractions;

public interface IUserSettingsService
{
    Task<ResultWrapper<UserSettingsDto?>> GetUserSettings();
    Task<ResultWrapper<bool>> UpdateUserSettings(UserSettingsDto userSettings);
}
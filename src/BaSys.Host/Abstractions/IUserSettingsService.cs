using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;

namespace BaSys.Host.Abstractions;

public interface IUserSettingsService
{
    Task<ResultWrapper<UserSettings?>> GetUserSettings(string userId);
    Task<ResultWrapper<bool>> UpdateUserSettings(UserSettings userSettings);
}
using BaSys.Common.Enums;
using BaSys.DAL.Models.App;

namespace BaSys.Host.DTO;

public class UserSettingsDto
{
    public UserSettingsDto()
    {
    }

    public UserSettingsDto(UserSettings userSettings)
    {
        Uid = userSettings.Uid;
        UserId = userSettings.UserId;
        Language = userSettings.Language;
    }
    
    public Guid Uid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Languages Language { get; set; }
}
﻿using BaSys.Common.Enums;
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
    public string UserName { get; set; }
    public Languages Language { get; set; }

    public UserSettings ToModel()
    {
        return new UserSettings
        {
            Uid = Uid,
            UserId = UserId,
            Language = Language
        };
    }
}
using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Models;

namespace BaSys.Host.DAL.ModelConfigurations;

public class UserSettingsConfiguration : DataModelConfiguration<UserSettings>
{
    public UserSettingsConfiguration()
    {
        Table("sys_user_settings");

        Column("uid").IsPrimaryKey();
        Column("UserId").IsRequired();
        Column("Language").IsRequired();
    }
}
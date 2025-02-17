using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Models;
using System.Data;

namespace BaSys.Host.DAL.ModelConfigurations;

public class UserSettingsConfiguration : DataModelConfiguration<UserSettings>
{
    public UserSettingsConfiguration()
    {
        Table("sys_user_settings");

        Column("uid").IsPrimaryKey();
        Column("userid").IsRequired();
        Column("language").ToType(DbType.Int32).IsRequired();

        OrderColumns();
    }
}
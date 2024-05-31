using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Models;

namespace BaSys.Host.DAL.ModelConfigurations;

public class UserGroupUserConfiguration : DataModelConfiguration<UserGroupUser>
{
    public UserGroupUserConfiguration()
    {
        Table("sys_user_group_users");

        Column("uid").IsPrimaryKey();
        Column("userGroupUid");
        Column("userUid");
    }
}
using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Models;

namespace BaSys.Host.DAL.ModelConfigurations;

public class UserGroupRoleConfiguration : DataModelConfiguration<UserGroupRole>
{
    public UserGroupRoleConfiguration()
    {
        Table("sys_user_group_roles");

        Column("uid").IsPrimaryKey();
        Column("userGroupUid");
        Column("roleUid");
    }
}
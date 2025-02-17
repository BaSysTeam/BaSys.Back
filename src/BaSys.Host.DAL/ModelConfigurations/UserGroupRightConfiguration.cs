using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Models;

namespace BaSys.Host.DAL.ModelConfigurations;

public class UserGroupRightConfiguration : DataModelConfiguration<UserGroupRight>
{
    public UserGroupRightConfiguration()
    {
        Table("sys_user_group_rights");

        Column("uid").IsPrimaryKey();
        Column("userGroupUid");
        Column("rightUid");
        Column("metaObjectKindUid");
        Column("metaObjectUid");

        OrderColumns();
    }
}
using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Models;

namespace BaSys.Host.DAL.ModelConfigurations;

public class UserGroupConfiguration : DataModelConfiguration<UserGroup>
{
    public UserGroupConfiguration()
    {
        Table("sys_user_groups");

        Column("uid").IsPrimaryKey();
        Column("name").MaxLength(128).IsRequired();
        Column("memo").MaxLength(256);
        Column("isDelete");
        Column("createDate");
    }
}
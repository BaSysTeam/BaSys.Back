namespace BaSys.DAL.Models.App;

public class UserGroupRole
{
    public Guid Uid { get; set; }
    public Guid UserGroupUid { get; set; }
    public Guid RoleUid { get; set; }
}
namespace BaSys.DAL.Models.App;

public class UserGroupUser
{
    public Guid Uid { get; set; }
    public Guid UserGroupUid { get; set; }
    public Guid UserUid { get; set; }
}
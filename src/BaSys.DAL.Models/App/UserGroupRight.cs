namespace BaSys.DAL.Models.App;

public class UserGroupRight
{
    public Guid Uid { get; set; }
    public Guid UserGroupUid { get; set; }
    public Guid RightUid { get; set; }
    public Guid MetaObjectKindUid { get; set; }
    public Guid? MetaObjectUid { get; set; }
}
using BaSys.Common.Abstractions;

namespace BaSys.DAL.Models.App;

public class UserGroupRight: SystemObjectBase
{
    public Guid UserGroupUid { get; set; }
    public Guid RightUid { get; set; }
    public Guid MetaObjectKindUid { get; set; }
    public Guid? MetaObjectUid { get; set; }
}
using BaSys.Common.Abstractions;

namespace BaSys.DAL.Models.App;

public class UserGroupRole: SystemObjectBase
{
    public Guid UserGroupUid { get; set; }
    public Guid RoleUid { get; set; }
}
using BaSys.Common.Abstractions;

namespace BaSys.DAL.Models.App;

public class UserGroupUser: SystemObjectBase
{
    public Guid UserGroupUid { get; set; }
    public Guid UserUid { get; set; }
}
using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;

namespace BaSys.Admin.Abstractions;

public interface IUserGroupService
{
    Task<ResultWrapper<IEnumerable<UserGroupDto>?>> GetUserGroupList();
    Task<ResultWrapper<bool>> CreateUserGroup(UserGroupDto userGroup);
    Task<ResultWrapper<bool>> DeleteUserGroup(Guid userGroupUid);
    Task<ResultWrapper<UserGroupDetailDto>> GetUserGroup(Guid userGroupUid);
    Task<ResultWrapper<UserGroupDetailDto>> SaveUserGroup(UserGroupDetailDto userGroup);
}
namespace BaSys.Admin.DTO;

public class UserGroupDetailDto : UserGroupDto
{
    public List<UserGroupUserDto>? Users { get; set; }
    public List<UserGroupRoleDto>? Roles { get; set; }
    public List<UserGroupRightDto>? GlobalRights { get; set; }
    public List<UserGroupRightDto>? Rights { get; set; }
}
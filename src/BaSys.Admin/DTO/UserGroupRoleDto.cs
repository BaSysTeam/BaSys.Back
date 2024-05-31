namespace BaSys.Admin.DTO;

public class UserGroupRoleDto
{
    public Guid Uid { get; set; }
    public Guid RoleUid { get; set; }
    public string? Name { get; set; }
    public bool IsChecked { get; set; }
}
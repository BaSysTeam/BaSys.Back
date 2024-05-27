namespace BaSys.Admin.DTO;

public class UserGroupUserDto
{
    public Guid Uid { get; set; }
    public Guid UserUid { get; set; }
    public string? UserName { get; set; }
    public bool IsChecked { get; set; }
}
namespace BaSys.Admin.DTO;

public class UserGroupRightDto
{
    public Guid Uid { get; set; }
    public Guid RightUid { get; set; }
    public Guid MetaObjectKindUid { get; set; }
    public Guid? MetaObjectUid { get; set; }
    public string? MetaObjectTitle { get; set; }
    public string? Name { get; set; }
    public bool IsChecked { get; set; }
}
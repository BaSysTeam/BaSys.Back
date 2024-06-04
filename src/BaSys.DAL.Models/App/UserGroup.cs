namespace BaSys.DAL.Models.App;

public class UserGroup
{
    public Guid Uid { get; set; }
    public required string Name { get; set; }
    public string? Memo { get; set; }
    public bool IsDelete { get; set; }
    public DateTime CreateDate { get; set; }
}
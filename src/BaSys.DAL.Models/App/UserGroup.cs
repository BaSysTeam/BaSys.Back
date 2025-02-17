using BaSys.Common.Abstractions;

namespace BaSys.DAL.Models.App;

public class UserGroup: SystemObjectBase
{
    public required string Name { get; set; }
    public string? Memo { get; set; }
    public bool IsDelete { get; set; }
    public DateTime CreateDate { get; set; }
}
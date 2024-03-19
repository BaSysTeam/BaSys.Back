using BaSys.Common.Enums;

namespace BaSys.SuperAdmin.DTO;

public class DbInfoRecordDto
{
    public int Id { get; set; }
    public string? AppId { get; set; }
    public string? Name { get; set; }
    public string? Title { get; set; }
    public DbKinds DbKind { get; set; }
    public string? ConnectionString { get; set; }
    public string? Memo { get; set; }
}
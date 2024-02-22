using BaSys.Common.Enums;

namespace BaSys.SuperAdmin.Data.Models;

public class DbInfoRecord
{
    public int Id { get; set; }
    public string AppId { get; set; }
    public string Title { get; set; }
    public DbKinds DbKind { get; set; }
    public string ConnectionString { get; set; }
    public string? Memo { get; set; }
    public bool IsDeleted { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is DbInfoRecord record &&
               Id == record.Id;
    }

    public override int GetHashCode()
    {
        return 2108858624 + Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Title}";
    }
}
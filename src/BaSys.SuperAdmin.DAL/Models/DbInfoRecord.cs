using BaSys.Common.Enums;

namespace BaSys.SuperAdmin.DAL.Models;

public class DbInfoRecord
{
    public int Id { get; set; }
    public string AppId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Title { get; set; }
    public DbKinds DbKind { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public string? Memo { get; set; }
    public bool IsDeleted { get; set; }

    public DbInfoRecord()
    {
        
    }
    
    public DbInfoRecord(DbInfoRecord source)
    {
        Fill(source);
    }
    
    public void Fill(DbInfoRecord source)
    {
        AppId = source.AppId;
        Name = source.Name;
        Title = source.Title;
        DbKind = source.DbKind;
        ConnectionString = source.ConnectionString;
        Memo = source.Memo;
        IsDeleted = source.IsDeleted;
    }
    
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
        return $"{Name}";
    }
}
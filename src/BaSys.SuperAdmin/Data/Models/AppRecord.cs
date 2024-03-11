namespace BaSys.SuperAdmin.Data.Models;

public class AppRecord
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Memo { get; set; }

    public AppRecord()
    {
        
    }

    public AppRecord(AppRecord source)
    {
        Fill(source);
    }

    public void Fill(AppRecord record)
    {
        Name = record.Name;
        Memo = record.Memo;
        Title = record.Title;
    }

    public override bool Equals(object? obj)
    {
        return obj is AppRecord description &&
               Id == description.Id;
    }

    public override int GetHashCode()
    {
        return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
    }

    public override string ToString()
    {
        return $"{Id}/{Name}";
    }
}
namespace BaSys.SuperAdmin.Data.Models;

public class AppRecord
{
    public string Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Memo { get; set; } = null;

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
        return $"{Id}/{Title}";
    }
}
using BaSys.Common.Enums;
using BaSys.SuperAdmin.DAL.Models;

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
    
    public DbInfoRecordDto()
    {
    }

    public DbInfoRecordDto(DbInfoRecord model)
    {
        Id = model.Id;
        AppId = model.AppId;
        Name = model.Name;
        Title = model.Title;
        DbKind = model.DbKind;
        ConnectionString = model.ConnectionString;
        Memo = model.Memo;
    }

    public DbInfoRecord ToModel()
    {
        return new DbInfoRecord
        {
            Id = Id,
            AppId = AppId ?? string.Empty,
            Name = Name ?? string.Empty,
            Title = Title,
            DbKind = DbKind,
            ConnectionString = ConnectionString ?? string.Empty,
            Memo = Memo
        };
    }
}
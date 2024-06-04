using BaSys.DAL.Models.App;

namespace BaSys.Admin.DTO;

public class UserGroupDto
{
    public UserGroupDto()
    {
    }

    public UserGroupDto(UserGroup model)
    {
        Uid = model.Uid;
        Name = model.Name;
        Memo = model.Memo;
        IsDelete = model.IsDelete;
        CreateDate = model.CreateDate;
    }
    
    public Guid Uid { get; set; }
    public string? Name { get; set; }
    public string? Memo { get; set; }
    public bool IsDelete { get; set; }
    public DateTime CreateDate { get; set; }

    public UserGroup ToModel()
    {
        return new UserGroup
        {
            Uid = Uid,
            Name = Name ?? string.Empty,
            Memo = Memo,
            IsDelete = IsDelete,
            CreateDate = CreateDate,
        };
    }
}
using System.Data;
using BaSys.Metadata.Models;

namespace BaSys.DTO.Core;

public class DataTypeDto
{
    public Guid Uid { get; }
    public string Kind { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsPrimitive { get; set; }
    public DbType DbType { get; set; }
    public Guid? ObjectKindUid { get; set; }
    public DataTypeDto()
    {
    }

    public DataTypeDto(DataType dataType)
    {
        Uid = dataType.Uid;
        Kind = dataType.Kind;
        Name = dataType.Name;
        Title = dataType.Title;
        IsPrimitive = dataType.IsPrimitive;
        DbType = dataType.DbType;
        ObjectKindUid = dataType.ObjectKindUid;
    }

   
}
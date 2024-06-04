using System.Data;
using BaSys.Metadata.Models;

namespace BaSys.Constructor.DTO;

public class DataTypeDto
{
    public DataTypeDto()
    {
    }

    public DataTypeDto(DataType dataType)
    {
        Uid = dataType.Uid;
        Title = dataType.Title;
        IsPrimitive = dataType.IsPrimitive;
        DbType = dataType.DbType;
        ObjectKindUid = dataType.ObjectKindUid;
    }
    
    public Guid Uid { get; }
    public string Title { get; set; } = string.Empty;
    public bool IsPrimitive { get; set; }
    public DbType DbType { get; set; }
    public Guid? ObjectKindUid { get; set; }
}
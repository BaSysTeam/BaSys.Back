namespace BaSys.Host.DTO;

public class GetFileListRequestDto
{
    public Guid MetaObjectKindUid {get; set;}
    public Guid MetaObjectUid { get; set; }
    public string? ObjectUid { get; set; }
}
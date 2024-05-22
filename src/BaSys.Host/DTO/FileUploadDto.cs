namespace BaSys.Host.DTO;

public class FileUploadDto
{
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public byte[]? Data { get; set; }
    public bool IsImage { get; set; }
    public bool IsMainImage { get; set; }
    public Guid MetaObjectKindUid {get; set;}
    public Guid MetaObjectUid { get; set; }
    public string? ObjectUid { get; set; }
}
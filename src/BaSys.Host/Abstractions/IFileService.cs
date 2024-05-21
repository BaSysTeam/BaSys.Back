using BaSys.Host.DTO;

namespace BaSys.Host.Abstractions;

public interface IFileService
{
    Task UploadFileAsync(FileUploadDto uploadDto);
    Task<bool> RemoveFileAsync(Guid metaObjectKindUid, Guid fileUid);
    Task<BaSys.Metadata.Models.FileInfo?> GetFileAsync(Guid metaObjectKindUid, Guid fileUid);
    Task<string?> GetImageBase64(Guid fileUid);
    Task<List<BaSys.Metadata.Models.FileInfo>?> GetAttachedFilesList(Guid metaObjectKindUid, Guid metaObjectUid, string objectUid);
}
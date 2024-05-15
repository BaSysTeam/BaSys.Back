using BaSys.Host.DTO;

namespace BaSys.Host.Abstractions;

public interface IFileService
{
    Task UploadFileAsync(FileUploadDto uploadDto);
    Task<bool> RemoveFileAsync(BaSys.Metadata.Models.FileInfo fileInfo);
    Task<List<BaSys.Metadata.Models.FileInfo>?> GetAttachedFilesList(Guid metaObjectKindUid, Guid metaObjectUid, string objectUid);
}
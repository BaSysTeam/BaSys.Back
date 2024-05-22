using BaSys.FileStorage.Models;

namespace BaSys.FileStorage.Abstractions;

public interface IFileStorageService
{
    Task<bool> UploadAsync(byte[] fileData, string encodedFileName, string contentType, Guid fileUid);
    Task<string?> DownloadBase64Async(Guid fileUid);
    Task<FileModel?> DownloadFileAsync(Guid fileUid);
    Task<bool> RemoveAsync(Guid fileUid);
}
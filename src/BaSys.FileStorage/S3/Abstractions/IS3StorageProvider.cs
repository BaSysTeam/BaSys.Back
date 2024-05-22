using Amazon.S3.Model;
using BaSys.FileStorage.Models;

namespace BaSys.FileStorage.S3.Abstractions;

public interface IS3StorageProvider
{
    Task<List<S3Bucket>> GetBucketListAsync();
    Task<List<S3Object>> GetObjectListAsync(string bucketName);
    Task<bool> CreateFolderAsync(string bucketName, string folderName);
    Task<bool> DeleteFileAsync(string bucketName, string s3FileName);
    Task<bool> DeleteFolderAsync(string bucketName, string folderName, bool deleteIfNotEmpty = false);
    Task<bool> UploadFileAsync(string bucketName, string s3FileName, byte[] fileArray, string contentType, string originFileName);
    Task<FileModel> DownloadFileAsync(string bucketName, string s3FileName);
}
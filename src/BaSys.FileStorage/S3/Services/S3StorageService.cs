using Amazon.S3;
using BaSys.FileStorage.Abstractions;
using BaSys.FileStorage.Models;
using BaSys.FileStorage.S3.Abstractions;
using BaSys.FileStorage.S3.Models;

namespace BaSys.FileStorage.S3.Services;

public class S3StorageService : IFileStorageService
{
    private readonly string? _bucketName;
    private readonly IS3StorageProvider _provider;
    private readonly Guid _dataBaseUid;
    
    public S3StorageService(S3ConnectionSettings connectionData, Guid dataBaseUid)
    {
        _dataBaseUid = dataBaseUid;
        
        var config = new AmazonS3Config
        {
            ServiceURL = connectionData.ServiceUrl
        };
        
        var s3Client = new AmazonS3Client(connectionData.AccessKeyId, connectionData.SecretAccessKey, config);
        _provider = new S3StorageProvider(s3Client);
        _bucketName = connectionData.BucketName;
    }
    
    public async Task<bool> UploadAsync(byte[] fileData, string encodedFileName, string contentType, Guid fileUid)
    {
        if (string.IsNullOrEmpty(_bucketName) || fileData.Length == 0 || fileUid == Guid.Empty)
            return false;
        
        var s3FileName = GetS3ObjectPath(fileUid);
        await _provider.UploadFileAsync(_bucketName, s3FileName, fileData, contentType, encodedFileName);

        return true;
    }

    public async Task<string?> DownloadBase64Async(Guid fileUid)
    {
        try
        {
            var fileModel = await DownloadFileAsync(fileUid);
            if (fileModel == null)
                return null;
            
            return ToBase64String(fileModel.Data!, fileModel.ContentType!);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<FileModel?> DownloadFileAsync(Guid fileUid)
    {
        if (string.IsNullOrEmpty(_bucketName) || fileUid == Guid.Empty)
            return null;
        
        try
        {
            var s3FileName = GetS3ObjectPath(fileUid);
            return await _provider.DownloadFileAsync(_bucketName, s3FileName);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<bool> RemoveAsync(Guid fileUid)
    {
        if (string.IsNullOrEmpty(_bucketName) || fileUid == Guid.Empty)
            return false;
        
        try
        {
            var s3FileName = GetS3ObjectPath(fileUid);
            await _provider.DeleteFileAsync(_bucketName, s3FileName);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    private string GetS3ObjectPath(Guid fileUid) => $"{GetFolderName()}/{GetFileName(fileUid)}";
    private string GetFolderName() => $"dbUid-{_dataBaseUid.ToString().Replace("-", "")}";
    private string GetFileName(Guid fileUid) => $"file-{fileUid.ToString().Replace("-", "")}";
    private string ToBase64String(byte[] data, string contentType) =>
        $"data:{contentType}; base64, " + Convert.ToBase64String(data);
}
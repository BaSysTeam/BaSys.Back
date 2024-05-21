using Amazon.S3;
using Amazon.S3.Model;
using BaSys.FileStorage.Models;
using BaSys.FileStorage.S3.Abstractions;

namespace BaSys.FileStorage.S3.Services;

public class S3StorageProvider : IS3StorageProvider
{
    private readonly IAmazonS3 _s3Client;

    public S3StorageProvider(IAmazonS3 s3S3Client)
    {
        _s3Client = s3S3Client;
    }
    
    public async Task<List<S3Bucket>> GetBucketListAsync()
    {
        var response = await _s3Client.ListBucketsAsync();
        return response.Buckets;
    }

    public async Task<List<S3Object>> GetObjectListAsync(string bucketName)
    {
        var objects = await _s3Client.ListObjectsAsync(bucketName);
        return objects.S3Objects;
    }

    public async Task<bool> CreateFolderAsync(string bucketName, string folderName)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = $"{folderName}/",
            FilePath = string.Empty
        };

        var response = await _s3Client.PutObjectAsync(request);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<bool> DeleteFileAsync(string bucketName, string s3FileName)
    {
        await _s3Client.DeleteObjectAsync(bucketName, s3FileName);
        return true;
    }

    public async Task<bool> DeleteFolderAsync(string bucketName, string folderName, bool deleteIfNotEmpty = false)
    {
        var s3FolderName = $"{folderName}/";

        var response = await _s3Client.ListObjectsAsync(bucketName, s3FolderName);
        if (response is null)
            return false;

        var folderObjects = response.S3Objects
            .Where(x => x.Key != s3FolderName)
            .ToList();

        if (!deleteIfNotEmpty && folderObjects.Any())
            return false;

        foreach (var item in folderObjects)
        {
            await _s3Client.DeleteObjectAsync(bucketName, item.Key);
        }

        await _s3Client.DeleteObjectAsync(bucketName, s3FolderName);
        return true;
    }

    public async Task<bool> UploadFileAsync(string bucketName, string s3FileName, byte[] fileArray, string contentType, string originFileName)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = s3FileName,
            InputStream = new MemoryStream(fileArray),
            AutoCloseStream = true,
            ContentType = contentType
        };

        var response = await _s3Client.PutObjectAsync(request);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<FileModel> DownloadFileAsync(string bucketName, string s3FileName)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = s3FileName,
        };

        using var response = await _s3Client.GetObjectAsync(request);
        using var stream = new MemoryStream();
        response.ResponseStream.CopyTo(stream);

        var originFileName = string.Empty;
        var metaKey = response.Metadata.Keys.FirstOrDefault(x => x.Contains("filename"));
        if (!string.IsNullOrEmpty(metaKey))
            originFileName = response.Metadata[metaKey];

        return new FileModel
        {
            Data = stream.ToArray(),
            Name = originFileName,
            ContentType = response.Headers.ContentType
        };
    }
}
using BaSys.Admin.Abstractions;
using BaSys.Common.Enums;
using BaSys.FileStorage.Abstractions;
using BaSys.FileStorage.S3.Models;
using BaSys.FileStorage.S3.Services;

namespace BaSys.FileStorage.Services;

public class FileStorageServiceFactory : IFileStorageServiceFactory
{
    private readonly IFileStorageConfigService _fileStorageConfigService;
    private readonly IAppConstantsService _appConstantsService;
    
    public FileStorageServiceFactory(IFileStorageConfigService fileStorageConfigService,
        IAppConstantsService appConstantsService)
    {
        _fileStorageConfigService = fileStorageConfigService;
        _appConstantsService = appConstantsService;
    }

    public async Task<IFileStorageService?> GetServiceAsync()
    {
        var configResult = await _fileStorageConfigService.GetFileStorageConfigAsync();
        if (!configResult.IsOK)
            return null;

        var appConstantsResult = await _appConstantsService.GetAppConstantsAsync();
        if (!appConstantsResult.IsOK)
            return null;

        var appConstants = appConstantsResult.Data;
        var config = configResult.Data;
        if (string.IsNullOrEmpty(config.S3ConnectionString))
            return null;
        
        var settings = new S3ConnectionSettings(config.S3ConnectionString);
        
        switch ((FileStorageKinds)config.StorageKind)
        {
            case FileStorageKinds.AmazonS3:
                return new S3StorageService(settings, appConstants.DataBaseUid);
            default:
                return null;
        }
    }
}
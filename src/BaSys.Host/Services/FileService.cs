using System.Data;
using System.Security.Claims;
using BaSys.Admin.Abstractions;
using BaSys.Common.Enums;
using BaSys.Constructor.Abstractions;
using BaSys.FileStorage.Abstractions;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DTO;
using BaSys.Metadata.Models;
using FileInfo = BaSys.Metadata.Models.FileInfo;

namespace BaSys.Host.Services;

public class FileService : IFileService
{
    private readonly IFileStorageServiceFactory _fileStorageServiceFactory;
    private readonly IMainConnectionFactory _connectionFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFileStorageConfigService _fileStorageConfigService;
    private readonly IMetaObjectKindsService _metaObjectKindsService;

    public FileService(IFileStorageServiceFactory fileStorageServiceFactory,
        IMainConnectionFactory connectionFactory,
        IHttpContextAccessor httpContextAccessor,
        IFileStorageConfigService fileStorageConfigService,
        IMetaObjectKindsService metaObjectKindsService)
    {
        _fileStorageServiceFactory = fileStorageServiceFactory;
        _connectionFactory = connectionFactory;
        _httpContextAccessor = httpContextAccessor;
        _fileStorageConfigService = fileStorageConfigService;
        _metaObjectKindsService = metaObjectKindsService;
    }

    public async Task UploadFileAsync(FileUploadDto uploadDto)
    {
        var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var transaction = connection.BeginTransaction();

        var config = (await _fileStorageConfigService.GetFileStorageConfigAsync())?.Data;
        if (config == null)
            return;

        var metaObjectKind = await _metaObjectKindsService.GetSettingsItemAsync(uploadDto.MetaObjectKindUid);
        if (!metaObjectKind.IsOK)
            return;

        var primaryKey = metaObjectKind.Data.StandardColumns.FirstOrDefault(x => x.IsPrimaryKey);
        if (primaryKey == null)
            return;

        if (!string.IsNullOrEmpty(uploadDto.MimeType) && uploadDto.MimeType.Contains("image"))
            uploadDto.IsImage = true;

        var fileUid = Guid.Empty;

        // save into AttachedFileInfo
        try
        {
            // int
            if (primaryKey.DataTypeUid == DataTypeDefaults.Int.Uid)
                fileUid = await SaveInt(connection, transaction, uploadDto, metaObjectKind.Data.Name,
                    (FileStorageKinds) config.StorageKind);

            // long
            // if (primaryKey.DataTypeUid == DataTypeDefaults.Long.Uid)
            //     fileUid = await SaveLong(connection, transaction, uploadDto, metaObjectKind.Data.Name, (FileStorageKinds) config.StorageKind);

            // Guid
            if (primaryKey.DataTypeUid == DataTypeDefaults.UniqueIdentifier.Uid)
                fileUid = await SaveGuid(connection, transaction, uploadDto, metaObjectKind.Data.Name,
                    (FileStorageKinds) config.StorageKind);

            // string
            if (primaryKey.DataTypeUid == DataTypeDefaults.String.Uid)
                fileUid = await SaveString(connection, transaction, uploadDto, metaObjectKind.Data.Name,
                    (FileStorageKinds) config.StorageKind);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }

        // save into external storage
// #if !DEBUG
        try
        {
            var service = await _fileStorageServiceFactory.GetServiceAsync();
            await service!.UploadAsync(uploadDto.Data!, uploadDto.FileName!, uploadDto.MimeType!, fileUid);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }
// #endif

        transaction.Commit();
    }

    public async Task<bool> RemoveFileAsync(Guid metaObjectKindUid, Guid fileUid)
    {
        var connection = _connectionFactory.CreateConnection();
        connection.Open();
        var transaction = connection.BeginTransaction();

        // ...
        var metaObjectKind = await _metaObjectKindsService.GetSettingsItemAsync(metaObjectKindUid);
        if (!metaObjectKind.IsOK)
            return false;

        var primaryKey = metaObjectKind.Data.StandardColumns.FirstOrDefault(x => x.IsPrimaryKey);
        if (primaryKey == null)
            return false;

        // save from AttachedFileInfo
        try
        {
            if (primaryKey.DataTypeUid == DataTypeDefaults.Int.Uid)
            {
                var provider = new AttachedFileInfoIntProvider(connection, metaObjectKind.Data.Name);
                await provider.DeleteAsync(fileUid, transaction);
            }
            // if (primaryKey.DataTypeUid == DataTypeDefaults.Long.Uid)
            //     await DeleteLong(connection, transaction, fileInfo.Uid, metaObjectKind.Data.Name);
            else if (primaryKey.DataTypeUid == DataTypeDefaults.String.Uid)
            {
                var provider = new AttachedFileInfoStringProvider(connection, metaObjectKind.Data.Name);
                await provider.DeleteAsync(fileUid, transaction);
            }
            else if (primaryKey.DataTypeUid == DataTypeDefaults.UniqueIdentifier.Uid)
            {
                var provider = new AttachedFileInfoGuidProvider(connection, metaObjectKind.Data.Name);
                await provider.DeleteAsync(fileUid, transaction);
            }
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return false;
        }

        // delete from external storage
// #if !DEBUG
        try
        {
            var service = await _fileStorageServiceFactory.GetServiceAsync();
            await service!.RemoveAsync(fileUid);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }
// #endif

        transaction.Commit();

        return true;
    }

    public async Task<FileInfo?> GetFileAsync(Guid metaObjectKindUid, Guid fileUid)
    {
        var connection = _connectionFactory.CreateConnection();

        var config = (await _fileStorageConfigService.GetFileStorageConfigAsync())?.Data;
        if (config == null)
            return null;

        var metaObjectKind = await _metaObjectKindsService.GetSettingsItemAsync(metaObjectKindUid);
        if (!metaObjectKind.IsOK)
            return null;

        var primaryKey = metaObjectKind.Data.StandardColumns.FirstOrDefault(x => x.IsPrimaryKey);
        if (primaryKey == null)
            return null;

        FileInfo? file = null;
        if (primaryKey.DataTypeUid == DataTypeDefaults.Int.Uid)
        {
            var provider = new AttachedFileInfoIntProvider(connection, metaObjectKind.Data.Name);
            var f = await provider.GetItemAsync(fileUid, null);
            file = new FileInfo
            {
                Uid = f.Uid,
                FileName = f.FileName,
                MimeType = f.MimeType,
                IsImage = f.IsImage,
                IsMainImage = f.IsMainImage,
                UploadDate = f.UploadDate,
                MetaObjectUid = f.MetaObjectUid,
                MetaObjectKindUid = f.MetaObjectKindUid
            };
        }
        // else if (primaryKey.DataTypeUid == DataTypeDefaults.Long.Uid)
        // {
        //     var provider = new AttachedFileInfoLongProvider(connection, metaObjectKind.Data.Name);
        //     var f = await provider.GetItemAsync(fileUid, null);
        // }
        else if (primaryKey.DataTypeUid == DataTypeDefaults.String.Uid)
        {
            var provider = new AttachedFileInfoStringProvider(connection, metaObjectKind.Data.Name);
            var f = await provider.GetItemAsync(fileUid, null);
            file = new FileInfo
            {
                Uid = f.Uid,
                FileName = f.FileName,
                MimeType = f.MimeType,
                IsImage = f.IsImage,
                IsMainImage = f.IsMainImage,
                UploadDate = f.UploadDate,
                MetaObjectUid = f.MetaObjectUid,
                MetaObjectKindUid = f.MetaObjectKindUid
            };
        }
        else if (primaryKey.DataTypeUid == DataTypeDefaults.UniqueIdentifier.Uid)
        {
            var provider = new AttachedFileInfoGuidProvider(connection, metaObjectKind.Data.Name);
            var f = await provider.GetItemAsync(fileUid, null);
            file = new FileInfo
            {
                Uid = f.Uid,
                FileName = f.FileName,
                MimeType = f.MimeType,
                IsImage = f.IsImage,
                IsMainImage = f.IsMainImage,
                UploadDate = f.UploadDate,
                MetaObjectUid = f.MetaObjectUid,
                MetaObjectKindUid = f.MetaObjectKindUid
            };
        }

        if (file == null)
            return null;

        var service = await _fileStorageServiceFactory.GetServiceAsync();
        var fileModel = await service!.DownloadFileAsync(fileUid);

        file.Data = fileModel?.Data;

        return file;
    }

    public async Task<List<FileInfo>?> GetAttachedFilesList(Guid metaObjectKindUid, Guid metaObjectUid,
        string objectUid)
    {
        var connection = _connectionFactory.CreateConnection();

        var config = (await _fileStorageConfigService.GetFileStorageConfigAsync())?.Data;
        if (config == null)
            return null;

        var metaObjectKind = await _metaObjectKindsService.GetSettingsItemAsync(metaObjectKindUid);
        if (!metaObjectKind.IsOK)
            return null;

        var primaryKey = metaObjectKind.Data.StandardColumns.FirstOrDefault(x => x.IsPrimaryKey);
        if (primaryKey == null)
            return null;

        List<FileInfo>? fileList = null;
        if (primaryKey.DataTypeUid == DataTypeDefaults.Int.Uid)
        {
            var provider = new AttachedFileInfoIntProvider(connection, metaObjectKind.Data.Name);
            fileList = await provider.GetAttachedFilesListAsync(metaObjectKindUid, metaObjectUid, objectUid);
        }
        // else if (primaryKey.DataTypeUid == DataTypeDefaults.Long.Uid)
        // {
        //     var provider = new AttachedFileInfoLongProvider(connection, metaObjectKind.Data.Name);
        //     fileList = await provider.GetAttachedFilesListAsync(metaObjectKindUid, metaObjectUid, objectUid);
        // }
        else if (primaryKey.DataTypeUid == DataTypeDefaults.String.Uid)
        {
            var provider = new AttachedFileInfoStringProvider(connection, metaObjectKind.Data.Name);
            fileList = await provider.GetAttachedFilesListAsync(metaObjectKindUid, metaObjectUid, objectUid);
        }
        else if (primaryKey.DataTypeUid == DataTypeDefaults.UniqueIdentifier.Uid)
        {
            var provider = new AttachedFileInfoGuidProvider(connection, metaObjectKind.Data.Name);
            fileList = await provider.GetAttachedFilesListAsync(metaObjectKindUid, metaObjectUid, objectUid);
        }

        // var service = await _fileStorageServiceFactory.GetServiceAsync();
        // foreach (var fileInfo in fileList ?? Enumerable.Empty<FileInfo>())
        // {
        //     if (fileInfo.MimeType.Contains("image"))
        //         fileInfo.Base64String = await service!.DownloadBase64Async(fileInfo.Uid);
        //     else
        //         fileInfo.Base64String = string.Empty;
        // }

        return fileList;
    }

    #region private methods

    private async Task<Guid> SaveInt(IDbConnection connection,
        IDbTransaction transaction,
        FileUploadDto uploadDto,
        string metaObjectKindName,
        FileStorageKinds storageKind)
    {
        if (!int.TryParse(uploadDto.ObjectUid, out var objectUid))
            throw new ApplicationException("ObjectUid is not set!");

        var provider = new AttachedFileInfoIntProvider(connection, metaObjectKindName);
        var fileUid = await provider.InsertDataAsync(new AttachedFileInfo<int>
        {
            MetaObjectKindUid = uploadDto.MetaObjectKindUid,
            MetaObjectUid = uploadDto.MetaObjectUid,
            ObjectUid = objectUid,
            FileName = uploadDto.FileName,
            MimeType = uploadDto.MimeType,
            IsImage = uploadDto.IsImage,
            IsMainImage = uploadDto.IsMainImage,
            StorageKind = storageKind,
            UserId = GetUserId(),
            UserName = GetUserName(),
            UploadDate = DateTime.UtcNow
        }, transaction);

        return fileUid;
    }

    private async Task<Guid> SaveLong(IDbConnection connection,
        IDbTransaction transaction,
        FileUploadDto uploadDto,
        string metaObjectKindName,
        FileStorageKinds storageKind)
    {
        if (!long.TryParse(uploadDto.ObjectUid, out var objectUid))
            throw new ApplicationException("ObjectUid is not set!");

        var provider = new AttachedFileInfoLongProvider(connection, metaObjectKindName);
        var fileUid = await provider.InsertDataAsync(new AttachedFileInfo<long>
        {
            MetaObjectKindUid = uploadDto.MetaObjectKindUid,
            MetaObjectUid = uploadDto.MetaObjectUid,
            ObjectUid = objectUid,
            FileName = uploadDto.FileName,
            MimeType = uploadDto.MimeType,
            IsImage = uploadDto.IsImage,
            IsMainImage = uploadDto.IsMainImage,
            StorageKind = storageKind,
            UserId = GetUserId(),
            UserName = GetUserName(),
            UploadDate = DateTime.UtcNow
        }, transaction);

        return fileUid;
    }

    private async Task<Guid> SaveString(IDbConnection connection,
        IDbTransaction transaction,
        FileUploadDto uploadDto,
        string metaObjectKindName,
        FileStorageKinds storageKind)
    {
        var provider = new AttachedFileInfoStringProvider(connection, metaObjectKindName);
        var fileUid = await provider.InsertDataAsync(new AttachedFileInfo<string>
        {
            MetaObjectKindUid = uploadDto.MetaObjectKindUid,
            MetaObjectUid = uploadDto.MetaObjectUid,
            ObjectUid = uploadDto.ObjectUid ?? string.Empty,
            FileName = uploadDto.FileName,
            MimeType = uploadDto.MimeType,
            IsImage = uploadDto.IsImage,
            IsMainImage = uploadDto.IsMainImage,
            StorageKind = storageKind,
            UserId = GetUserId(),
            UserName = GetUserName(),
            UploadDate = DateTime.UtcNow
        }, transaction);

        return fileUid;
    }

    private async Task<Guid> SaveGuid(IDbConnection connection,
        IDbTransaction transaction,
        FileUploadDto uploadDto,
        string metaObjectKindName,
        FileStorageKinds storageKind)
    {
        if (!Guid.TryParse(uploadDto.ObjectUid, out var objectUid))
            throw new ApplicationException("ObjectUid is not set!");

        var provider = new AttachedFileInfoGuidProvider(connection, metaObjectKindName);
        var fileUid = await provider.InsertDataAsync(new AttachedFileInfo<Guid>
        {
            MetaObjectKindUid = uploadDto.MetaObjectKindUid,
            MetaObjectUid = uploadDto.MetaObjectUid,
            ObjectUid = objectUid,
            FileName = uploadDto.FileName,
            MimeType = uploadDto.MimeType,
            IsImage = uploadDto.IsImage,
            IsMainImage = uploadDto.IsMainImage,
            StorageKind = storageKind,
            UserId = GetUserId(),
            UserName = GetUserName(),
            UploadDate = DateTime.UtcNow
        }, transaction);

        return fileUid;
    }

    private string GetUserId()
        => _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    private string GetUserName()
        => _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;

    #endregion
}
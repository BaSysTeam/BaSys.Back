using BaSys.Admin.DTO;
using BaSys.Common.DTO;
using BaSys.Common.Infrastructure;

namespace BaSys.Admin.Abstractions;

public interface IFileStorageConfigService
{
    Task<ResultWrapper<FileStorageConfigDto>> GetFileStorageConfigAsync();
    Task<ResultWrapper<bool>> UpdateFileStorageConfigAsync(FileStorageConfigDto fileStorageConfig);
    ResultWrapper<List<EnumValuesDto>> GetStorageKinds();
}
using BaSys.Common.Infrastructure;
using BaSys.DTO.Constructor;
using BaSys.DTO.Metadata;
using BaSys.Metadata.DTOs;

namespace BaSys.Core.Abstractions;

public interface IMetaObjectsService
{
    Task<ResultWrapper<MetaObjectStorableSettingsDto>> GetSettingsItemAsync(string kindName, string objectName);
    Task<ResultWrapper<MetaObjectListDto>> GetKindListAsync(string kindName);
    Task<ResultWrapper<int>> CreateMetaObjectAsync(CreateMetaObjectDto createDto);
    Task<ResultWrapper<int>> UpdateSettingsItemAsync(MetaObjectStorableSettingsDto settingsDto);
    Task<ResultWrapper<List<MetaObjectStorableSettingsDto>>> GetMetaObjectsAsync(string kindName);
    Task<ResultWrapper<int>> DeleteAsync(string kindName, string objectName);
}
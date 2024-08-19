using BaSys.Common.Infrastructure;
using BaSys.DTO.Metadata;

namespace BaSys.Core.Abstractions;

public interface IMetaObjectsService
{
    Task<ResultWrapper<MetaObjectStorableSettingsDto>> GetSettingsItemAsync(string kindName, string objectName);
    Task<ResultWrapper<int>> UpdateSettingsItemAsync(MetaObjectStorableSettingsDto settingsDto);
    Task<ResultWrapper<List<MetaObjectStorableSettingsDto>>> GetMetaObjectsAsync(string kindName);
}
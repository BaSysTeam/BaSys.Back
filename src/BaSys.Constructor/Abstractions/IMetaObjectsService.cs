using BaSys.Common.Infrastructure;
using BaSys.Metadata.DTOs;

namespace BaSys.Constructor.Abstractions
{
    public interface IMetaObjectsService
    {
        Task<ResultWrapper<MetaObjectStorableSettingsDto>> GetSettingsItemAsync(string kindName, string objectName);
    }
}

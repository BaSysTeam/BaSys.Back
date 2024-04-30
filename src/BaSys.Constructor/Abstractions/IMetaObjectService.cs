using BaSys.Common.Infrastructure;
using BaSys.Metadata.DTOs;

namespace BaSys.Constructor.Abstractions
{
    public interface IMetaObjectService
    {
        Task<ResultWrapper<MetaObjectStorableSettingsDto>> GetSettingsItemAsync(string kindName, string objectName);
    }
}

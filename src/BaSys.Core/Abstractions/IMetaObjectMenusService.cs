using BaSys.Common.Infrastructure;
using BaSys.DTO.Constructor;
using BaSys.DTO.Metadata;
using BaSys.Metadata.Models.MenuModel;

namespace BaSys.Core.Abstractions
{
    public interface IMetaObjectMenusService
    {
     
        Task<ResultWrapper<MetaObjectListDto>> GetKindListAsync();
        Task<ResultWrapper<MetaObjectMenuSettings>> GetSettingsItemAsync(string objectName);
        Task<ResultWrapper<int>> CreateAsync(MetaObjectMenuSettings settings);
        Task<ResultWrapper<int>> UpdateSettingsItemAsync(MetaObjectMenuSettings settings);
        Task<ResultWrapper<int>> DeleteAsync(string objectName);
    }
}

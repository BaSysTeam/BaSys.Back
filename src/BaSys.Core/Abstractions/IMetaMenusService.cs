using BaSys.Common.Infrastructure;
using BaSys.DTO.Constructor;
using BaSys.DTO.Metadata;
using BaSys.Metadata.Models.MenuModel;

namespace BaSys.Core.Abstractions
{
    public interface IMetaMenusService
    {
     
        Task<ResultWrapper<MetaObjectListDto>> GetKindListAsync();
        Task<ResultWrapper<MenuSettings>> GetSettingsItemAsync(string objectName);
        Task<ResultWrapper<int>> CreateAsync(MenuSettings settings);
        Task<ResultWrapper<int>> UpdateSettingsItemAsync(MenuSettings settings);
        Task<ResultWrapper<int>> DeleteAsync(string objectName);
    }
}

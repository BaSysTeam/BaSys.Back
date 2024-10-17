using BaSys.Common.Infrastructure;
using BaSys.DTO.App;

namespace BaSys.App.Abstractions
{
    public interface IMenusService
    {
        Task<ResultWrapper<IEnumerable<MenuGroupDto>>> GetCollectionAsync();
    }
}

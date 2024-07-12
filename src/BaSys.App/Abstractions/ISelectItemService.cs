using BaSys.Common.Infrastructure;
using BaSys.Metadata.Models;

namespace BaSys.App.Abstractions
{
    public interface ISelectItemService
    {
        Task<ResultWrapper<IEnumerable<SelectItem>>> GetColllectionAsync(Guid dataTypeUid);
        Task<ResultWrapper<SelectItem>> GetItemAsync(Guid dataTypeUid, string uid);
    }
}

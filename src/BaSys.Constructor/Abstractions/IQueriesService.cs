using BaSys.Common.Infrastructure;
using BaSys.DTO.Core;

namespace BaSys.Constructor.Abstractions
{
    public interface IQueriesService
    {
        Task<ResultWrapper<DataTableDto>> ExecuteAsync(SelectQueryModelDto queryModel);
    }
}

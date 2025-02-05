using BaSys.Common.Infrastructure;
using System.Threading.Tasks;

namespace BaSys.Common.Abstractions
{
    public interface IQueryHandlerBase<TQuery, TResult>
    {
        Task<ResultWrapper<TResult>> ExecuteAsync(TQuery query);
    }
}

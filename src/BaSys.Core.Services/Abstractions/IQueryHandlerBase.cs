using BaSys.Common.Infrastructure;

namespace BaSys.Core.Features.Abstractions
{
    public interface IQueryHandlerBase<TQuery, TResult>
    {
        Task<ResultWrapper<TResult>> ExecuteAsync(TQuery query);
    }
}

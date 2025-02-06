using BaSys.Common.Abstractions;
using BaSys.Common.Infrastructure;

namespace BaSys.Core.Features.Abstractions
{
    public abstract class QueryHandlerBase<TQuery, TResult> : IQueryHandlerBase<TQuery, TResult>
    {
        public abstract Task<ResultWrapper<TResult>> ExecuteAsync(TQuery query);
        protected abstract Task<ResultWrapper<TResult>> ExecuteQueryAsync(TQuery query);

    }
}

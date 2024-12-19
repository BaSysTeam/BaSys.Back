using BaSys.Common.Infrastructure;

namespace BaSys.Core.Features.Abstractions
{
    public interface ICommandHandlerBase<TCommand, TResult>
    {
        Task<ResultWrapper<TResult>> ExecuteAsync(TCommand command);
    }
}

using BaSys.Common.Infrastructure;
using System.Threading.Tasks;

namespace BaSys.Common.Abstractions
{
    public interface ICommandHandlerBase<TCommand, TResult>
    {
        Task<ResultWrapper<TResult>> ExecuteAsync(TCommand command);
    }
}

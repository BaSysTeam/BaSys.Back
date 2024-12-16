using BaSys.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Core.Features.Abstractions
{
    public abstract class CommandHandlerBase<TCommand, TResult>
    {
        public async Task<ResultWrapper<TResult>> ExecuteAsync(TCommand command)
        {
            var result = new ResultWrapper<TResult>();

            try
            {
                result = await ExecuteCommandAsync(command);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot execute command: {nameof(command)}. Message: {ex.Message}.", ex.StackTrace);
            }

            return result;
        }

        protected abstract Task<ResultWrapper<TResult>> ExecuteCommandAsync(TCommand command);
    }
}

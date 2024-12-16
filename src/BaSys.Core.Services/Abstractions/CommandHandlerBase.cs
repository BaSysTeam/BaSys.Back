using BaSys.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Core.Features.Abstractions
{
    public abstract class CommandHandlerBase<TCommand, TResult> : ICommandHandlerBase<TCommand, TResult>
    {
        public abstract Task<ResultWrapper<TResult>> ExecuteAsync(TCommand command);
        protected abstract Task<ResultWrapper<TResult>> ExecuteCommandAsync(TCommand command, IDbTransaction transaction);

    }
}

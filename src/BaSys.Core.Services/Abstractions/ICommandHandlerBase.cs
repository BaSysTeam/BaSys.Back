using BaSys.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Core.Features.Abstractions
{
    public interface ICommandHandlerBase<TCommand, TResult>
    {
        Task<ResultWrapper<TResult>> ExecuteAsync(TCommand command);
    }
}

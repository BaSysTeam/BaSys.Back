using BaSys.App.Features.DataObjectRecords.Commands;
using BaSys.Common.Infrastructure;

namespace BaSys.App.Abstractions
{
    public interface ICreateRecordsCommandHandler
    {
        Task<ResultWrapper<bool>> ExecuteAsync(CreateRecordsCommand command);
    }
}

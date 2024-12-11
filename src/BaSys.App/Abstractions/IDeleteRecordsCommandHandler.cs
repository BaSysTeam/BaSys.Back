using BaSys.App.Features.DataObjectRecords.Commands;
using BaSys.Common.Infrastructure;

namespace BaSys.App.Abstractions
{
    public interface IDeleteRecordsCommandHandler
    {
        Task<ResultWrapper<bool>> ExecuteAsync(DeleteRecordsCommand command);
    }
}

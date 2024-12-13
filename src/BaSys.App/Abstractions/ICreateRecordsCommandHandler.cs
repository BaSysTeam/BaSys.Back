using BaSys.App.Features.DataObjectRecords.Commands;
using BaSys.Common.Infrastructure;
using BaSys.Logging.InMemory;

namespace BaSys.App.Abstractions
{
    public interface ICreateRecordsCommandHandler
    {
        Task<ResultWrapper<List<InMemoryLogMessage>>> ExecuteAsync(CreateRecordsCommand command);
    }
}

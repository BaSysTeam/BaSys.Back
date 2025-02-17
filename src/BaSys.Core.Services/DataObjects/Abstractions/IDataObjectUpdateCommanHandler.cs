using BaSys.Common.Abstractions;
using BaSys.DTO.App;
using BaSys.Logging.InMemory;
using System.Data;

namespace BaSys.Core.Features.DataObjects.Abstractions
{
    public interface IDataObjectUpdateCommanHandler: ICommandHandlerBase<DataObjectSaveDto, List<InMemoryLogMessage>>
    {
        IDataObjectUpdateCommanHandler SetUp(IDbConnection connection);
    }
}

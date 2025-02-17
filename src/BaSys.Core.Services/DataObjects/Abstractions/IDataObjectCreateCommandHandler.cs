using BaSys.Common.Abstractions;
using BaSys.DTO.App;
using System.Data;

namespace BaSys.Core.Features.DataObjects.Abstractions
{
    public interface IDataObjectCreateCommandHandler: ICommandHandlerBase<DataObjectSaveDto, string>
    {
        IDataObjectCreateCommandHandler SetUp(IDbConnection connection);
    }
}

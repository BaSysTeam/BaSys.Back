using BaSys.Common.Abstractions;
using BaSys.Core.Features.DataObjects.Queries;
using BaSys.DTO.App;

namespace BaSys.Core.Features.Abstractions
{
    public interface IDataObjectRegistratorRouteQueryHandler: IQueryHandlerBase<DataObjectRegistratorRouteQuery, DataObjectRouteDto>
    {
    }
}

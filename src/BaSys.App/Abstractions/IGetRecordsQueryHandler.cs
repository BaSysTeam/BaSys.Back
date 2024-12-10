using BaSys.App.Features.DataObjectRecords.Queries;
using BaSys.Common.Infrastructure;
using BaSys.DTO.Core;

namespace BaSys.App.Abstractions
{
    public interface IGetRecordsQueryHandler
    {
        Task<ResultWrapper<DataTableDto>> ExecuteAsync(GetRecordsQuery query);

    }
}

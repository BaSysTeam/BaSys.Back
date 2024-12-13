using BaSys.App.Features.DataObjectRecords.Queries;
using BaSys.App.Models.DataObjectRecordsDialog;
using BaSys.Common.Infrastructure;

namespace BaSys.App.Abstractions
{
    public interface IGetRecordsDialogModelQueryHandler
    {
        Task<ResultWrapper<DataObjectRecordsDialogViewModel>> ExecuteAsync(GetRecordsDialogModelQuery query);
    }
}

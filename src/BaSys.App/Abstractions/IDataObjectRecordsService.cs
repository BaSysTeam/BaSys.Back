using BaSys.App.Models.DataObjectRecordsDialog;
using BaSys.Common.Infrastructure;
using BaSys.DTO.Core;

namespace BaSys.App.Abstractions
{
    public interface IDataObjectRecordsService
    {
        Task<ResultWrapper<DataObjectRecordsDialogViewModel>> GetModelAsync(string kind, string name, string uid);
        Task<ResultWrapper<DataTableDto>> GetRecordsAsync(string kind, string name, string objectUid, Guid registerUid);
    }
}

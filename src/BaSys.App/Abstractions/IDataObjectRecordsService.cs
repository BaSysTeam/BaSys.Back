using BaSys.App.Models.DataObjectRecordsDialog;
using BaSys.Common.Infrastructure;

namespace BaSys.App.Abstractions
{
    public interface IDataObjectRecordsService
    {
        Task<ResultWrapper<DataObjectRecordsDialogViewModel>> GetModelAsync(string kind, string name, string uid);
    }
}

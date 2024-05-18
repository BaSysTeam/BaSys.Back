using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;


namespace BaSys.App.Abstractions
{
    public interface IDataObjectsService
    {
        Task<ResultWrapper<List<DataObject>>> GetCollectionAsync(string kindName, string objectName);
    }
}

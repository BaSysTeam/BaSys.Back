using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;
using BaSys.DTO.App;


namespace BaSys.App.Abstractions
{
    public interface IDataObjectsService
    {
        Task<ResultWrapper<List<DataObject>>> GetCollectionAsync(string kindName, string objectName);
        Task<ResultWrapper<int>> InsertAsync(DataObjectDto dto);
    }
}

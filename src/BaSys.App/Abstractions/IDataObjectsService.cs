using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;
using BaSys.DTO.App;


namespace BaSys.App.Abstractions
{
    public interface IDataObjectsService
    {
        Task<ResultWrapper<DataObjectListDto>> GetCollectionAsync(string kindName, string objectName);
        Task<ResultWrapper<DataObjectWithMetadataDto>> GetItemAsync(string kindName, string objectName, string uid);
        Task<ResultWrapper<int>> InsertAsync(DataObjectSaveDto dto);
        Task<ResultWrapper<int>> UpdateAsync(DataObjectSaveDto dto);
        Task<ResultWrapper<int>> DeleteItemAsync(string kindName, string objectName, string uid);
    }
}

using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;
using BaSys.DTO.App;
using BaSys.Logging.InMemory;


namespace BaSys.App.Abstractions
{
    public interface IDataObjectsService
    {
        Task<ResultWrapper<DataObjectListDto>> GetCollectionAsync(string kindName, string objectName);
        Task<ResultWrapper<DataObjectWithMetadataDto>> GetItemAsync(string kindName, string objectName, string uid);
        Task<ResultWrapper<DataObjectDetailsTableDto>> GetDetailsTableAsync(string kindName, string objectName, string uid, string tableName);
        Task<ResultWrapper<string>> InsertAsync(DataObjectSaveDto dto);
        Task<ResultWrapper<List<InMemoryLogMessage>>> UpdateAsync(DataObjectSaveDto dto);
        Task<ResultWrapper<int>> DeleteItemAsync(string kindName, string objectName, string uid);
    }
}

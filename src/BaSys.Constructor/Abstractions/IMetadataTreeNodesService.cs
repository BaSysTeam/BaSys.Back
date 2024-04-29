using BaSys.Common.Infrastructure;
using BaSys.Metadata.DTOs;
using BaSys.Metadata.Models;

namespace BaSys.Constructor.Abstractions
{
    public interface IMetadataTreeNodesService
    {
        Task<ResultWrapper<List<MetadataTreeNodeDto>>> GetStandardNodesAsync();
        Task<ResultWrapper<int>> InsertAsync(MetadataTreeNodeDto dto);
        Task<ResultWrapper<int>> InsertMetaObjectAsync(CreateMetaObjectDto dto);
        Task<ResultWrapper<int>> DeleteAsync(Guid uid);
        Task<ResultWrapper<List<MetadataTreeNodeDto>>> GetChildrenAsync(Guid uid);
    }
}

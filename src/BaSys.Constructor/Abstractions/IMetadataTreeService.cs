using BaSys.Common.Infrastructure;
using BaSys.Metadata.DTOs;
using BaSys.Metadata.Models;

namespace BaSys.Constructor.Abstractions
{
    public interface IMetadataTreeService
    {
        Task<ResultWrapper<List<MetadataTreeNode>>> GetDefaultNodesAsync(string dbName);
        Task<ResultWrapper<int>> AddAsync(MetadataGroupDto dto, string dbName);
        Task<ResultWrapper<int>> DeleteAsync(string uid, string dbName);
        Task<ResultWrapper<List<MetadataTreeNode>>> GetChildrenAsync(string uid, string dbName);
    }
}

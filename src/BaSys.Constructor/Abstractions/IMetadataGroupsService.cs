using BaSys.Common.Infrastructure;
using BaSys.Metadata.DTOs;

namespace BaSys.Constructor.Abstractions
{
    public interface IMetadataGroupsService
    {
        Task<int> InsertAsync(MetadataGroupDto dto, string dbName);
        Task<bool> HasChildrenAsync(Guid parentUid, string dbName);
        Task<int> DeleteAsync(Guid uid, string dbName);
        Task<List<MetadataGroupDto>> GetChildrenAsync(Guid parentUid, string dbName);
    }
}

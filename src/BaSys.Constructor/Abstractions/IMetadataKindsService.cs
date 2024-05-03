using BaSys.Common.Infrastructure;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.Constructor.Abstractions
{
    public interface IMetadataKindsService
    {
        Task<ResultWrapper<MetadataKindSettings>> GetSettingsItemAsync(Guid uid);
        Task<ResultWrapper<MetadataKindSettings>> GetSettingsItemByNameAsync(string name);
        Task<ResultWrapper<IEnumerable<MetadataKind>>> GetCollectionAsync();
        Task<ResultWrapper<IEnumerable<MetadataKindSettings>>> GetSettingsCollection();
        Task<ResultWrapper<MetadataKindSettings>> InsertSettingsAsync(MetadataKindSettings settings);
        Task<ResultWrapper<int>> UpdateSettingsAsync(MetadataKindSettings settings);
        Task<ResultWrapper<int>> DeleteAsync(Guid uid);

    }
}

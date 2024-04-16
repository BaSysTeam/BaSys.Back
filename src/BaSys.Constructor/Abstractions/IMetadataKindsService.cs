using BaSys.Common.Infrastructure;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.Constructor.Abstractions
{
    public interface IMetadataKindsService
    {
        void SetUp(string? dbName);
        Task<ResultWrapper<MetadataKindSettings>> GetSettingsItemAsync(Guid uid, IDbTransaction? transaction);
        Task<ResultWrapper<IList<MetadataKindSettings>>> GetSettingsCollectionAsync(IDbTransaction? transaction);
        Task<ResultWrapper<int>> InsertSettingsAsync(MetadataKindSettings settings, IDbTransaction? transaction);
        Task<ResultWrapper<int>> UpdateSettingsAsync(MetadataKindSettings settings, IDbTransaction? transaction);
        Task<ResultWrapper<int>> DeleteAsync(Guid uid, IDbTransaction? transaction);

    }
}

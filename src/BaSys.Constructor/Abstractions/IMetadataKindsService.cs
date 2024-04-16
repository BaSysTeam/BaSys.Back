using BaSys.Common.Infrastructure;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.Constructor.Abstractions
{
    public interface IMetadataKindsService
    {
        Task<ResultWrapper<int>> InsertSettingsAsync(MetadataKindSettings settings, IDbTransaction transaction);
    }
}

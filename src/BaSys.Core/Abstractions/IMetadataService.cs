using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.Core.Abstractions
{
    public interface IMetadataService
    {
        void SetUp(IDbConnection connection);
        Task<IEnumerable<MetaObjectKind>> GetAllKindsAsync(IDbTransaction? transaction);
        Task<MetaObjectKind> GetKindAsync(Guid uid, IDbTransaction? transaction);
        Task<MetaObjectKindSettings> GetKindSettingsAsync(Guid uid, IDbTransaction? transaction);
        Task<MetaObjectKind> GetKindByNameAsync(string name, IDbTransaction? transaction);
        Task<MetaObjectKindSettings> GetKindSettingsByNameAsync(string name, IDbTransaction? transaction);

    }
}

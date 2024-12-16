using BaSys.Host.DAL.Abstractions;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.Core.Features.Abstractions
{
    public interface IMetadataReader
    {
        void SetUp(ISystemObjectProviderFactory providerFactory);
        Task<IEnumerable<MetaObjectKind>> GetAllKindsAsync(IDbTransaction? transaction);
        Task<MetaObjectKind> GetKindAsync(Guid uid, IDbTransaction? transaction);
        Task<MetaObjectKindSettings> GetKindSettingsAsync(Guid uid, IDbTransaction? transaction);
        Task<MetaObjectKind> GetKindByNameAsync(string kindName, IDbTransaction? transaction);
        Task<MetaObjectKindSettings> GetKindSettingsByNameAsync(string kindName, IDbTransaction? transaction);

        Task<IEnumerable<MetaObjectStorable>> GetAllMetaObjectsAsync(IDbTransaction? transaction);
        Task<MetaObjectStorable> GetMetaObjectByNameAsync(string kindName, string objectName, IDbTransaction? transaction);
        Task<MetaObjectStorableSettings> GetMetaObjectSettingsByNameAsync(string kindName, string objectName, IDbTransaction? transaction);

        Task<List<DataType>> GetAllDataTypesAsync(IDbTransaction? transaction);
        Task<IDataTypesIndex> GetIndexAsync(IDbTransaction? transaction);

    }
}

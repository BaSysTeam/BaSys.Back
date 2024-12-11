using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.Core.Services
{
    public sealed class MetadataService : IMetadataService
    {
        private ISystemObjectProviderFactory _providerFactory;
        private MetaObjectKindsProvider _kindProvider;
        private readonly List<MetaObjectKind> _kinds;
        private readonly List<MetaObjectStorable> _metaObjects;

        public MetadataService()
        {
            _kinds = new List<MetaObjectKind>();
            _metaObjects = new List<MetaObjectStorable>();
        }

        public void SetUp(ISystemObjectProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();
        }

        public async Task<IEnumerable<MetaObjectKind>> GetAllKindsAsync(IDbTransaction? transaction)
        {
            await UpdateAllKindsIfNecessaryAsync(transaction);
            return _kinds.ToList();
        }

        public async Task<MetaObjectKind> GetKindAsync(Guid uid, IDbTransaction? transaction)
        {
            await UpdateAllKindsIfNecessaryAsync(transaction);
            return _kinds.FirstOrDefault(x => x.Uid == uid);
        }

        public async Task<MetaObjectKind> GetKindByNameAsync(string name, IDbTransaction transaction)
        {
            await UpdateAllKindsIfNecessaryAsync(transaction);
            return _kinds.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<MetaObjectKindSettings> GetKindSettingsAsync(Guid uid, IDbTransaction? transaction)
        {
            var item = await GetKindAsync(uid, transaction);
            return item?.ToSettings();
        }

        public async Task<MetaObjectKindSettings> GetKindSettingsByNameAsync(string name, IDbTransaction? transaction)
        {
            var item = await GetKindByNameAsync(name, transaction);
            return item?.ToSettings();
        }

        public async Task<IEnumerable<MetaObjectStorable>> GetAllMetaObjectsAsync(IDbTransaction? transaction)
        {
            await UpdateAllMetaObjectsIfNecessaryAsync(transaction);
            return _metaObjects.ToList();
        }

        public async Task<MetaObjectStorable> GetMetaObjectByNameAsync(string kindName, string objectName, IDbTransaction transaction)
        {
            await UpdateAllMetaObjectsIfNecessaryAsync(transaction);

            var kind = await GetKindByNameAsync(kindName, transaction);

            if (kind == null)
            {
                return null;
            }

            var metaObject = _metaObjects.FirstOrDefault(x => x.MetaObjectKindUid == kind.Uid
            && x.Name.Equals(objectName, StringComparison.CurrentCultureIgnoreCase));

            return metaObject;
        }

        public async Task<MetaObjectStorableSettings> GetMetaObjectSettingsByNameAsync(string kindName, string objectName, IDbTransaction transaction)
        {
            var metaObject = await GetMetaObjectByNameAsync(kindName, objectName, transaction);

            return metaObject?.ToSettings();
        }

        private async Task UpdateAllMetaObjectsIfNecessaryAsync(IDbTransaction? transaction)
        {
            await UpdateAllKindsIfNecessaryAsync(transaction);

            if (!_metaObjects.Any())
            {
                _metaObjects.Clear();

                foreach (var kind in _kinds)
                {
                    var metaObjectProvider = _providerFactory.CreateMetaObjectStorableProvider(kind.Name);
                    var metaObjects = await metaObjectProvider.GetCollectionAsync(transaction);

                    _metaObjects.AddRange(metaObjects);
                }
            }
        }

        private async Task UpdateAllKindsIfNecessaryAsync(IDbTransaction? transaction)
        {
            if (!_kinds.Any())
            {
                _kinds.Clear();
                _kinds.AddRange(await _kindProvider.GetCollectionAsync(transaction));
            }
        }


    }
}

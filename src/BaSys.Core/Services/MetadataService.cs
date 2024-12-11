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
        private IDbConnection _connection;
        private readonly List<MetaObjectKind> _allKinds;

        public MetadataService(ISystemObjectProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;

            _allKinds = new List<MetaObjectKind>();
        }

        public void SetUp(IDbConnection connection)
        {
            _connection = connection;
            _providerFactory.SetUp(_connection);
            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();

        }

        public async Task<IEnumerable<MetaObjectKind>> GetAllKindsAsync(IDbTransaction? transaction)
        {
            await UpdateAllKindsIfNecessaryAsync(transaction);
            return _allKinds.ToList();
        }

        public async Task<MetaObjectKind> GetKindAsync(Guid uid, IDbTransaction? transaction)
        {
            await UpdateAllKindsIfNecessaryAsync(transaction);
            return _allKinds.FirstOrDefault(x => x.Uid == uid);
        }

        public async Task<MetaObjectKind> GetKindByNameAsync(string name, IDbTransaction transaction)
        {
            await UpdateAllKindsIfNecessaryAsync(transaction);
            return _allKinds.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
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

        private async Task UpdateAllKindsIfNecessaryAsync(IDbTransaction? transaction)
        {
            if (!_allKinds.Any())
            {
                _allKinds.Clear();
                _allKinds.AddRange(await _kindProvider.GetCollectionAsync(transaction));
            }
        }


    }
}

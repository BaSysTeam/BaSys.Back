using BaSys.Core.Features.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Helpers;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.Core.Features.MetaObjects.Services
{
    public sealed class MetadataReader : IMetadataReader
    {
        private ISystemObjectProviderFactory _providerFactory;
        private MetaObjectKindsProvider _kindProvider;
        private readonly List<MetaObjectKind> _kinds;
        private readonly List<MetaObjectStorable> _metaObjects;

        public MetadataReader()
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

        public async Task<List<DataType>> GetAllDataTypesAsync(IDbTransaction? transaction)
        {
            await UpdateAllMetaObjectsIfNecessaryAsync(transaction);

            var primitiveDataTypes = new PrimitiveDataTypes();
            var allDataTypes = DataTypeDefaults.AllTypes().ToList();

            foreach (var metaObjectKind in _kinds.Where(x => x.IsReference))
            {
                var metaObjects = _metaObjects.Where(x => x.MetaObjectKindUid == metaObjectKind.Uid).ToList();
                var dataTypes = metaObjects.Select(x => ToDataType(x, metaObjectKind, primitiveDataTypes));
                allDataTypes.AddRange(dataTypes);
            }

            return allDataTypes;
        }

        public async Task<IDataTypesIndex> GetIndexAsync(IDbTransaction? transaction)
        {
            var allDataTypes = await GetAllDataTypesAsync(transaction);
            var dataTypeIndex = new DataTypesIndex(allDataTypes);

            return dataTypeIndex;
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

        private DataType ToDataType(MetaObjectStorable metaObject, MetaObjectKind metaObjectKind, PrimitiveDataTypes primitiveDataTypes)
        {
            var dataType = new DataType(metaObject.Uid)
            {
                Title = $"{metaObjectKind.Title}.{metaObject.Title}",
                IsPrimitive = false,
                DbType = GetDbType(metaObject, primitiveDataTypes),
                ObjectKindUid = metaObjectKind.Uid
            };

            return dataType;
        }

        private DbType GetDbType(MetaObjectStorable metaObject, PrimitiveDataTypes primitiveDataTypes)
        {
            var dbType = DbType.String; // Default type.
            var settings = metaObject.ToSettings();
            if (settings == null)
                return dbType;

            var headerTable = settings.Header;
            if (headerTable == null)
                return dbType;

            var primaryKeyColumn = headerTable.Columns.FirstOrDefault(x => x.PrimaryKey);
            if (primaryKeyColumn == null)
                return dbType;

            var dataType = primitiveDataTypes.GetDataType(primaryKeyColumn.DataTypeUid);
            if (dataType == null)
                return dbType;

            return dataType.DbType;
        }


    }
}

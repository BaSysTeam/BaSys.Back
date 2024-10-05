using System.Data;
using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using BaSys.Metadata.Helpers;

namespace BaSys.Core.Services;

public class DataTypesService : IDataTypesService, IDisposable
{
    private  ISystemObjectProviderFactory _providerFactory;
    private  IDbConnection _connection;

    public DataTypesService(ISystemObjectProviderFactory providerFactory)
    {
        _providerFactory = providerFactory;
    }

    public void SetUp(IDbConnection connection)
    {
        _connection = connection;
        _providerFactory.SetUp(_connection);

    }

    public async Task<List<DataType>> GetAllDataTypesAsync(IDbTransaction transaction)
    {
        var primitiveDataTypes = new PrimitiveDataTypes();
        var allDataTypes = DataTypeDefaults.AllTypes().ToList();
        var metaObjectKindProvider = _providerFactory.Create<MetaObjectKindsProvider>();

        var metaObjectKinds = await metaObjectKindProvider.GetCollectionAsync(transaction);

        foreach (var metaObjectKind in metaObjectKinds.Where(x => x.IsReference))
        {
            var provider = _providerFactory.CreateMetaObjectStorableProvider(metaObjectKind.Name);
            var metaObjects = await provider.GetCollectionAsync(transaction);
            var dataTypes = metaObjects.Select(x => ToDataType(x, metaObjectKind, primitiveDataTypes));
            allDataTypes.AddRange(dataTypes);
        }

        return allDataTypes;
    }

    public async Task<IDataTypesIndex> GetIndexAsync(IDbTransaction transaction)
    {
        var allDataTypes = await GetAllDataTypesAsync(transaction);

        var dataTypeIndex = new DataTypesIndex(allDataTypes);

        return dataTypeIndex;
    }

    public void Dispose()
    {
        if (_connection != null)
            _connection.Dispose();
    }

    #region private methods
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
    #endregion
}
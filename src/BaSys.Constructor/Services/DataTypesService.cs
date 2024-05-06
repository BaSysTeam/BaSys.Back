using System.Data;
using BaSys.Constructor.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Metadata.Models;

namespace BaSys.Constructor.Services;

public class DataTypesService : IDataTypesService, IDisposable
{
    private readonly IMetaObjectKindsService _metaObjectKindsService;
    private readonly ISystemObjectProviderFactory _providerFactory;
    private readonly IDbConnection? _connection;

    public DataTypesService(IMetaObjectKindsService metaObjectKindsService,
        ISystemObjectProviderFactory providerFactory,
        IMainConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
        _metaObjectKindsService = metaObjectKindsService;
        _providerFactory = providerFactory;
        _providerFactory.SetUp(_connection);
    }
    
    public async Task<List<DataType>> GetAllDataTypes()
    {
        var allDataTypes = DataTypeDefaults.AllTypes().ToList();

        var result = await _metaObjectKindsService.GetCollectionAsync();
        if (result.IsOK)
        {
            foreach (var metaObjectKind in result.Data.Where(x => x.IsReference))
            {
                var provider = _providerFactory.CreateMetaObjectStorableProvider(metaObjectKind.Name);
                var metaObjects = await provider.GetCollectionAsync(null);
                var dataTypes = metaObjects.Select(x => ToDataType(x, metaObjectKind));
                allDataTypes.AddRange(dataTypes);
            }
        }
        
        return allDataTypes;
    }
    
    public void Dispose()
    {
        if (_connection != null)
            _connection.Dispose();
    }

    #region private methods
    private DataType ToDataType(MetaObjectStorable metaObject, MetaObjectKind metaObjectKind)
    {
        var dataType = new DataType(metaObject.Uid)
        {
            Title = $"{metaObjectKind.Title}.{metaObject.Title}",
            IsPrimitive = false,
            DbType = GetDbType(metaObject),
            ObjectKindUid = metaObjectKind.Uid
        };

        return dataType;
    }

    private DbType GetDbType(MetaObjectStorable metaObject)
    {
        // ToDo: default type?
        var dbType = DbType.Binary;
        var settings = metaObject.ToSettings();
        if (settings == null)
            return dbType;
        
        var headerTable = settings.Header;
        if (headerTable == null)
            return dbType;

        var primaryKeyColumn = headerTable.Columns.FirstOrDefault(x => x.PrimaryKey);
        if (primaryKeyColumn == null)
            return dbType;

        // ToDo: column type?
        return dbType;
    }
    #endregion
}
using System.Data;
using BaSys.Host.DAL.Abstractions;

namespace BaSys.Host.DAL.TableManagers;

public class AttachedFileInfoManagerFactory
{
    public TableManagerBase? GetTableManager(IDbConnection connection, string kindName, Guid dataTypeUid)
    {
        var dataType = DataTypeDefaults.AllTypes().FirstOrDefault(x => x.Uid == dataTypeUid);
        var type = typeof(AttachedFileInfoManager<>).MakeGenericType(dataType.Type);
        var tableManager = Activator.CreateInstance(type, connection, kindName) as TableManagerBase;
        
        return tableManager;
    }
}
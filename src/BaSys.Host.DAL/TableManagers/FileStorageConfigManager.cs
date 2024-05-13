using System.Data;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;

namespace BaSys.Host.DAL.TableManagers;

public class FileStorageConfigManager : TableManagerBase
{
    public FileStorageConfigManager(IDbConnection connection) : base(connection, new FileStorageConfigConfiguration())
    {
    }
}
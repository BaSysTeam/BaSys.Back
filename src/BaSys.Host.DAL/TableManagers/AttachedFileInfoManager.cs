using System.Data;
using BaSys.FluentQueries.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;

namespace BaSys.Host.DAL.TableManagers;

public class AttachedFileInfoManager<T> : TableManagerBase
{
    public AttachedFileInfoManager(IDbConnection connection, string kindName) : base(connection, new AttachedFileInfoConfiguration<T>(kindName))
    {
    }
}
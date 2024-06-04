using System.Data;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;

namespace BaSys.Host.DAL.TableManagers;

public class UserGroupRightManager : TableManagerBase
{
    public UserGroupRightManager(IDbConnection connection) : base(connection, new UserGroupRightConfiguration())
    {
    }
}
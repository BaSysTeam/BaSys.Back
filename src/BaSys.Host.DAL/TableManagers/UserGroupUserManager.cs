using System.Data;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;

namespace BaSys.Host.DAL.TableManagers;

public class UserGroupUserManager : TableManagerBase
{
    public UserGroupUserManager(IDbConnection connection) : base(connection, new UserGroupUserConfiguration())
    {
    }
}
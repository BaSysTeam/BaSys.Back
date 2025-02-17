using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using System.Data;

namespace BaSys.Host.DAL.TableManagers
{
    public sealed class WorkflowTriggerManager : TableManagerBase
    {
        public WorkflowTriggerManager(IDbConnection connection) : base(connection, new WorkflowTriggerConfiguration())
        {
        }
    }
}

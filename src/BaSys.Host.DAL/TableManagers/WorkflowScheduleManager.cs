using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.ModelConfigurations;
using System.Data;

namespace BaSys.Host.DAL.TableManagers
{
    public sealed class WorkflowScheduleManager: TableManagerBase
    {
        public WorkflowScheduleManager(IDbConnection connection): base(connection, new WorkflowScheduleRecordConfiguration())
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Workflows.Enums
{
    /// <summary>
    /// WorkflowCore has no Running status for workflow. 
    /// That is why we define own enum with Runnig status.
    /// </summary>
    public enum BaSysWorkflowStatuses
    {
        Waiting = 0,
        Running = 1,
        Suspended = 2,
        Complete = 3,
        Terminated = 4,
    }
}

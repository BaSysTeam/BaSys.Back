using BaSys.Metadata.Models.WorkflowModel;
using BaSys.SuperAdmin.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Workflows.Commands
{
    public sealed class WorkflowScheduleStartCommand
    {
        public DbInfoRecord? DbInfoRecord { get; set; }
        public List<WorkflowScheduleRecord> ScheduleRecords { get; set; } = new List<WorkflowScheduleRecord>();
    }
}

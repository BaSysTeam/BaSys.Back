using BaSys.Common.Abstractions;
using System;

namespace BaSys.Metadata.Models.WorkflowModel
{
    public sealed class WorkflowScheduleRecord: SystemObjectBase
    {
        public Guid WorkflowUid { get; set; }
        public string CronExpression { get; set; }
        public string Memo { get; set; }
        public bool IsActive { get; set; }

        public void CopyFrom(WorkflowScheduleRecord source)
        {
            WorkflowUid = source.WorkflowUid;
            CronExpression = source.CronExpression;
            Memo = source.Memo;
            IsActive = source.IsActive;
        }

        public override string ToString()
        {
            return $"{IsActive}. {WorkflowUid}. {CronExpression}";
        }
    }

}

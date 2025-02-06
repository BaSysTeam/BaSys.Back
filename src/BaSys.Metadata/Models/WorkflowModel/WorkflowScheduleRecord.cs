using System;

namespace BaSys.Metadata.Models.WorkflowModel
{
    public sealed class WorkflowScheduleRecord
    {
        public Guid Uid { get; set; }
        public Guid WorkflowUid { get; set; }
        public string CronExpression { get; set; }
        public string Memo { get; set; }
        public bool IsActive { get; set; }

        public void CopyFrom(WorkflowScheduleRecord source)
        {
            WorkflowUid = source.Uid;
            CronExpression = source.CronExpression;
            Memo = source.Memo;
            IsActive = source.IsActive;
        }

        public void BeforeSave()
        {
            if (Uid == Guid.Empty)
            {
                Uid = Guid.NewGuid();
            }
        }

        public override string ToString()
        {
            return $"{IsActive} / {WorkflowUid} / {CronExpression}";
        }
    }


}

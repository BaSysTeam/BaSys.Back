using BaSys.FluentQueries.Models;
using BaSys.Metadata.Models.WorkflowModel;

namespace BaSys.Host.DAL.ModelConfigurations
{
    public class WorkflowScheduleRecordConfiguration: DataModelConfiguration<WorkflowScheduleRecord>
    {
        public WorkflowScheduleRecordConfiguration()
        {
            Table("sys_workflow_schedule");

            Column("Uid").IsPrimaryKey();
            Column("WorkflowUid").IsRequired();
            Column("CronExpression").IsRequired().MaxLength(50);
            Column("IsActive").IsRequired();
            Column("Memo").IsOptional().MaxLength(300);

            OrderColumns();

        }
    }
}

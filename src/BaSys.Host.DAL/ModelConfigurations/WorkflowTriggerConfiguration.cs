using BaSys.FluentQueries.Models;
using BaSys.Metadata.Models.WorkflowModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.ModelConfigurations
{
    public sealed class WorkflowTriggerConfiguration: DataModelConfiguration<WorkflowTrigger>
    {
        public WorkflowTriggerConfiguration()
        {
            Table("sys_workflow_triggers");

            Column("Uid").IsPrimaryKey();
            Column("MetaObjectKindUid").IsRequired();
            Column("MetaObjectUid").IsRequired();
            Column("EventUid").IsRequired();
            Column("WorkflowUid").IsRequired();
            Column("IsActive").IsRequired();
            Column("Memo").IsOptional().MaxLength(300);
        }
    }
}

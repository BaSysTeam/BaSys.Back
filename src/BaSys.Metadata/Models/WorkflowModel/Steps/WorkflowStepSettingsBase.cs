using BaSys.Metadata.Models.WorkflowModel.Abstractions;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models.WorkflowModel.Steps
{
    [MessagePackObject(keyAsPropertyName: true)]
    [Union(0, typeof(MessageStepSettings))]
    [Union(1, typeof(SleepStepSettings))]
    public abstract class WorkflowStepSettingsBase : IWorkflowStepSettings
    {
        public Guid Uid { get; set; }
        public Guid? PreviousStepUid { get; set; }
        public abstract IWorkflowStepKind Kind { get; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public bool IsActive { get; set; } = true;

        public override string ToString()
        {
            return $"{Title}/{Name}";
        }
    }
}

using BaSys.Metadata.Models.WorkflowModel.Abstractions;
using BaSys.Metadata.Models.WorkflowModel.StepKinds;
using MessagePack;

namespace BaSys.Metadata.Models.WorkflowModel.Steps
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class SleepStepSettings : WorkflowStepSettingsBase
    {
        public override IWorkflowStepKind Kind => new SleepStepKind();
        public string Delay { get; set; } = string.Empty;
    }
}

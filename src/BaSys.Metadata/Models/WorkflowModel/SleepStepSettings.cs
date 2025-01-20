using MessagePack;

namespace BaSys.Metadata.Models.WorkflowModel
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class SleepStepSettings: WorkflowStepSettingsBase
    {
        public override IWorkflowStepKind Kind => new SleepStepKind();
        public int Delay { get; set; }
    }
}

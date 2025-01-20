using MessagePack;

namespace BaSys.Metadata.Models.WorkflowModel
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MessageStepSettings: WorkflowStepSettingsBase
    {
        public override IWorkflowStepKind Kind => new MessageStepKind();
        public string Message { get; set; }
    }
}

using BaSys.Metadata.Models.WorkflowModel.Abstractions;
using BaSys.Metadata.Models.WorkflowModel.StepKinds;
using MessagePack;

namespace BaSys.Metadata.Models.WorkflowModel.Steps
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MessageStepSettings : WorkflowStepSettingsBase
    {
        public override IWorkflowStepKind Kind => new MessageStepKind();
        public string Message { get; set; }
    }
}

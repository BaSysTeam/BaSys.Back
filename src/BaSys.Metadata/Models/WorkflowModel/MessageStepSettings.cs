namespace BaSys.Metadata.Models.WorkflowModel
{
    public sealed class MessageStepSettings: WorkflowStepSettingsBase
    {
        public override IWorkflowStepKind Kind => new MessageStepKind();
        public string Message { get; set; }
    }
}

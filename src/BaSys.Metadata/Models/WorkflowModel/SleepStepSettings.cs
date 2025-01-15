namespace BaSys.Metadata.Models.WorkflowModel
{
    public sealed class SleepStepSettings: WorkflowStepSettingsBase
    {
        public override IWorkflowStepKind Kind => new SleepStepKind();
        public int Delay { get; set; }
    }
}

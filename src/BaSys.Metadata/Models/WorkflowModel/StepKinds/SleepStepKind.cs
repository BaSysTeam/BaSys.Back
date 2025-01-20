using System;

namespace BaSys.Metadata.Models.WorkflowModel.StepKinds
{
    public sealed class SleepStepKind : WorkflowStepKindBase
    {
        public override Guid Uid => Guid.Parse("{2A5ACA6D-2C41-4BD2-9E03-B35DF7DAB23E}");
        public override string Name => "sleep";
    }
}

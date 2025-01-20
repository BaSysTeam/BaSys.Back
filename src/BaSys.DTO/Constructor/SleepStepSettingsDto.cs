using BaSys.Metadata.Models.WorkflowModel.Steps;

namespace BaSys.DTO.Constructor
{
    public sealed class SleepStepSettingsDto : WorkflowStepSettingsBaseDto
    {
        public string Delay { get; set; } = string.Empty;

        public SleepStepSettingsDto()
        {

        }
        public SleepStepSettingsDto(SleepStepSettings source) : base(source)
        {
            Delay = source.Delay.ToString();
        }

        public override WorkflowStepSettingsBase? ToModel()
        {
            var settings = base.ToModel();

            if (settings is SleepStepSettings sleepStep)
            {
                sleepStep.Delay = Delay;
            }

            return settings;
        }
    }
}

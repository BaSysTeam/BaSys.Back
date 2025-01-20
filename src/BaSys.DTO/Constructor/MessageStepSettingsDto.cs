using BaSys.Metadata.Models.WorkflowModel.Steps;

namespace BaSys.DTO.Constructor
{
    public sealed class MessageStepSettingsDto: WorkflowStepSettingsBaseDto
    {
        public string Message { get; set; } = string.Empty;

        public MessageStepSettingsDto()
        {
            
        }

        public MessageStepSettingsDto(MessageStepSettings source):base(source)
        {
            Message = source.Message;
        }

        public override WorkflowStepSettingsBase? ToModel()
        {
            var settings = base.ToModel();

            if (settings is MessageStepSettings messageStep)
            {
                messageStep.Message = Message;
            }

            return settings;
        }
    }
}

using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Metadata.Models.WorkflowModel.StepKinds;
using BaSys.Metadata.Models.WorkflowModel.Steps;
using System.Text.Json;

namespace BaSys.DTO.Constructor
{
    public sealed class WorkflowSettingsDto
    {
        public string Uid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public List<object> Steps { get; set; } = new List<object>();

        public WorkflowSettingsDto()
        {

        }

        public WorkflowSettingsDto(WorkflowSettings source)
        {
            Uid = source.Uid.ToString();
            Name = source.Name;
            Title = source.Title;
            Memo = source.Memo;
            IsActive = source.IsActive;

            Steps.Clear();
            foreach (var step in source.Steps)
            {
                if (step is SleepStepSettings sleepStep)
                {
                    var stepDto = new SleepStepSettingsDto(sleepStep);
                    Steps.Add(stepDto);
                }
                else if (step is MessageStepSettings messageStep)
                {
                    var stepDto = new MessageStepSettingsDto(messageStep);
                    Steps.Add(stepDto);
                }
            }

        }

        public WorkflowSettings ToModel()
        {
            var settings = new WorkflowSettings()
            {
                Title = Title,
                Name = Name,
                Memo = Memo,
                IsActive = IsActive
            };

            if (Guid.TryParse(Uid, out var guidValue))
            {
                settings.Uid = guidValue;
            }

            foreach (var stepObject in Steps)
            {
                if (stepObject is WorkflowStepSettingsBaseDto stepDto)
                {
                    var stepSettings = stepDto.ToModel();
                    if (stepSettings != null)
                    {
                        settings.Steps.Add(stepSettings);
                    }
                }
            }

            return settings;
        }

        public WorkflowSettingsDto Parse()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var settingsDto = new WorkflowSettingsDto();

            settingsDto.Uid = Uid;
            settingsDto.Name = Name;
            settingsDto.Title = Title;
            settingsDto.Memo = Memo;
            settingsDto.IsActive = IsActive;

            foreach(var stepObject in Steps)
            {
                if (stepObject is JsonElement jElement)
                {
                    var kindUid = jElement.GetProperty("kindUid").GetGuid();
                    var kindName = jElement.GetProperty("kindName").GetString();

                    WorkflowStepSettingsBaseDto? stepSettings = null;

                    if (kindUid == WorkflowStepKinds.Sleep.Uid)
                    {
                        stepSettings = jElement.Deserialize<SleepStepSettingsDto>(options);
                    }
                    else if (kindUid == WorkflowStepKinds.Message.Uid)
                    {
                        stepSettings = jElement.Deserialize<MessageStepSettingsDto>(options);
                    }
                    else
                    {
                        throw new ArgumentException($"Unknown step kind: {kindName}/{kindUid}");
                    }

                    if (stepSettings != null)
                    {
                        settingsDto.Steps.Add(stepSettings);
                    }

                }
            }

            return settingsDto;
        }
    }
}

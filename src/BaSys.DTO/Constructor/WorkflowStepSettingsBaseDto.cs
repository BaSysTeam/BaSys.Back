using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Metadata.Models.WorkflowModel.StepKinds;
using BaSys.Metadata.Models.WorkflowModel.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.Constructor
{
    public abstract class WorkflowStepSettingsBaseDto
    {
        public string Uid { get; set; } = string.Empty;
        public string PreviousStepUid { get; set; } = string.Empty;
        public string KindUid { get; set; } = string.Empty;
        public string KindName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        protected WorkflowStepSettingsBaseDto()
        {
            
        }

        protected WorkflowStepSettingsBaseDto(WorkflowStepSettingsBase source)
        {
            Uid = source.Uid.ToString();
            PreviousStepUid = source.PreviousStepUid?.ToString() ?? string.Empty;
            KindUid = source.Kind.Uid.ToString();
            KindName = source.Kind.Name;
            Title = source.Title;
            Name = source.Name;
            IsActive = source.IsActive;
        }

        public virtual WorkflowStepSettingsBase? ToModel()
        {
            WorkflowStepSettingsBase settings = null;

            var kindUid = Guid.Parse(KindUid);
            if(kindUid == WorkflowStepKinds.Sleep.Uid)
            {
                settings = new SleepStepSettings();
            }
            else if (kindUid == WorkflowStepKinds.Message.Uid)
            {
                settings = new MessageStepSettings();
            }
            else
            {
                throw new ArgumentException($"Unknown step kind: {KindName}/{kindUid}");
            }

         
            if (settings != null)
            {
                settings.Name = Name;
                settings.Title = Title;
                settings.Memo = Memo;
                settings.IsActive = IsActive;

                if (Guid.TryParse(Uid, out var uidValue))
                {
                    settings.Uid = uidValue;
                }

                if (!string.IsNullOrWhiteSpace(PreviousStepUid)
                    && Guid.TryParse(PreviousStepUid, out var previousStepUid))
                {
                    settings.PreviousStepUid = previousStepUid;
                }
            }

            return settings;

        }

        public override string ToString()
        {
            return $"{Title}/{Name}";
        }
    }
}

using BaSys.Metadata.Models.WorkflowModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public WorkflowSettingsDto(WorkflowSettings settings)
        {
            Uid = settings.Uid.ToString();
            Name = settings.Name;
            Title = settings.Title;
            Memo = settings.Memo;
            IsActive = settings.IsActive;
            
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

            return settings;
        }
    }
}

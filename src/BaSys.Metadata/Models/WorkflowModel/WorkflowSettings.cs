using MessagePack;
using System;
using System.Collections.Generic;

namespace BaSys.Metadata.Models.WorkflowModel
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class WorkflowSettings
    {
        public Guid Uid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public long Version { get; set; }

        public List<WorkflowStepSettingsBase> Steps { get; set; } = new List<WorkflowStepSettingsBase>();

        public void CopyFrom(WorkflowSettings source)
        {
            Name = source.Name;
            Title = source.Title;
            Memo = source.Memo;
            IsActive = source.IsActive;
            Version = source.Version;
        }

        public override string ToString()
        {
            return $"{Title}/{Name}";
        }
    }
}

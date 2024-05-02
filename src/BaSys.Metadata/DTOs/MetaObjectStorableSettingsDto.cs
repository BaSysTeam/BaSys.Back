using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.DTOs
{
    public sealed class MetaObjectStorableSettingsDto
    {
        public string MetaObjectKindTitle { get; set; } = string.Empty;

        public string Uid { get; set; } = string.Empty;
        public Guid MetaObjectKindUid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public long Version { get; set; }
        public bool IsActive { get; set; }
        public List<MetaObjectTable> Tables { get; set; } = new();

        public MetaObjectStorableSettingsDto()
        {

        }

        public MetaObjectStorableSettingsDto(MetaObjectStorableSettings settings, MetadataKindSettings kindSettings)
        {
            Uid = settings.Uid.ToString();
            Title = settings.Title;
            Name = settings.Name;
            Memo = settings.Memo;
            IsActive = settings.IsActive;

            MetaObjectKindUid = kindSettings.Uid;
            MetaObjectKindTitle = kindSettings.Title;

            Tables = settings.Tables;

        }
    }
}

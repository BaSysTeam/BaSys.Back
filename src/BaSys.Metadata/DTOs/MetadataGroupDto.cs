using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Metadata.DTOs
{
    public class MetadataGroupDto
    {
        public string Uid { get; set; } = string.Empty;
        public string ParentUid { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsStandard { get; set; }

        public MetadataGroupDto()
        {

        }

        public MetadataGroupDto(MetadataGroup model)
        {
            Uid = model.Uid.ToString();
            ParentUid = model.ParentUid.ToString();
            Title = model.Title;
            IconClass = model.IconClass;
            Memo = model.Memo;
            IsStandard = model.IsStandard;
        }

        public MetadataGroup ToModel()
        {
            var model = new MetadataGroup
            {
                IconClass = IconClass,
                IsStandard = IsStandard,
                Memo = Memo,
                Title = Title
            };

            if (Guid.TryParse(Uid, out var uid))
                model.Uid = uid;
            if (Guid.TryParse(ParentUid, out var parentUid))
                model.ParentUid = parentUid;

            return model;
        }
    }
}

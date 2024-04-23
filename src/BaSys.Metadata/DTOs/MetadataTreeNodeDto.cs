using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.DTOs
{
    public sealed class MetadataTreeNodeDto
    {
        public Guid Key { get; set; }
        public Guid? ParentKey { get; set; }
        public Guid? MetadataKindUid { get; set; }
        public Guid? MetadataObjectUid { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
        public string Memo { get; set; }
        public bool IsStandard { get; set; }
        public bool IsGroup { get; set; }
        public bool Leaf { get; set; }
        public IEnumerable<MetadataTreeNodeDto> Children { get; set; } = new List<MetadataTreeNodeDto>();

        public MetadataTreeNodeDto()
        {
        }

        public MetadataTreeNodeDto(MetadataTreeNode model)
        {
            Key = model.Uid;
            ParentKey = model.ParentUid;
            MetadataKindUid = model.MetadataKindUid;
            MetadataObjectUid = model.MetadataObjectUid;
            Label = model.Title;
            Icon = model.IconClass;
            Memo = model.Memo;
            IsStandard = model.IsStandard;
            IsGroup = model.IsGroup;
        }

        public MetadataTreeNode ToModel()
        {
            return new MetadataTreeNode
            {
                IconClass = Icon,
                IsGroup = IsGroup,
                IsStandard = IsStandard,
                MetadataKindUid = MetadataKindUid,
                MetadataObjectUid = MetadataObjectUid,
                ParentUid = ParentKey,
                Title = Label,
                Memo = Memo,
                Uid = Key
            };
        }
    }
}

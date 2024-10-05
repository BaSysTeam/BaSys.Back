using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.Metadata
{
    public sealed class MetaObjectDto
    {
        public string Uid { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public MetaObjectDto()
        {
            
        }

        public MetaObjectDto(MetaObjectStorable source)
        {
            Uid = source.Uid.ToString();
            Name = source.Name;
            Title = source.Title;
            Memo = source.Memo;
            IsActive = source.IsActive;
        }
    }
}

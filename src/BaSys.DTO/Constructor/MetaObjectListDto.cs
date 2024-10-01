using BaSys.DTO.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.Constructor
{
    public sealed class MetaObjectListDto
    {
        public string Title { get; set; } = string.Empty;
        public List<MetaObjectDto> Items { get; set; } = new List<MetaObjectDto>();
    }
}

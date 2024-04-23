using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    public sealed class MetaObjectTable
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public bool IsHeader { get; set; }
        public List<MetaObjectTableColumn> Columns { get; set; } = new List<MetaObjectTableColumn>();   
    }
}

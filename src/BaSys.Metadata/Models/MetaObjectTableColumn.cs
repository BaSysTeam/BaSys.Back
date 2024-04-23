using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    public class MetaObjectTableColumn
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid DataTypeUid { get; set; }
        public int StringLength { get; set; }
        public int NumberDigits { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Required { get; set; }
        public bool Unique { get; set; }
    }
}

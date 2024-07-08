using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    public sealed class ControlKind
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public List<DataType> AppliesFor { get; set; } = new List<DataType>();
    }
}

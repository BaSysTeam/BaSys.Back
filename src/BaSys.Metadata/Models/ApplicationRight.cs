using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    public sealed class ApplicationRight
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Memo { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsMain { get; set; }
    }
}

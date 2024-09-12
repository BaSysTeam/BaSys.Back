using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.Core
{
    public sealed class DataTableColumnDto
    {
        public string Name { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Name}/{DataType}";
        }
    }
}

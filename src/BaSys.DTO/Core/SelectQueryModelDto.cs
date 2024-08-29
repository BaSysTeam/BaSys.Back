using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.Core
{
    public sealed class SelectQueryModelDto
    {
        public int Top { get; set; }
        public string FromExpression { get; set; } = string.Empty;
        public string WhereExpression { get; set; } = string.Empty;
        public string OrderByExpression { get; set; } = string.Empty;
    }
}

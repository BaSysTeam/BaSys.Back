using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.Core
{
    public sealed class QueryParameterDto
    {
        public string Name { get; set; } = string.Empty;
        public object? Value { get; set; }
        public DbType? DbType { get; set; }
    }
}

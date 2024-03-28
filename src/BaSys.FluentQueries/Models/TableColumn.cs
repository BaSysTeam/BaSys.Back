using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public sealed class TableColumn
    {
        public string Name { get; set; } = string.Empty;
        public DbType DbType { get; set; }
        public int StringLength { get; set; }
        public int NumberDigits { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Required { get; set; }
        public bool Unique { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DAL.Models.App
{
    public sealed class DataObjectDetailsTable
    {
        public Guid Uid { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<DataObjectDetailsTableRow> Rows { get; set; } = new List<DataObjectDetailsTableRow>();
    }
}

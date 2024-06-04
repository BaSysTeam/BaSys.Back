using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    [MemoryPackable]
    public sealed partial class MetaObjectTable
    {

        public Guid Uid { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public List<MetaObjectTableColumn> Columns { get; set; } = new List<MetaObjectTableColumn>();

        [MemoryPackIgnore]
        public MetaObjectTableColumn PrimaryKey => Columns.FirstOrDefault(x => x.PrimaryKey);

        public static MetaObjectTable HeaderTable()
        {
            var table = new MetaObjectTable()
            {
                Title = "Header",
                Name = "header"
            };

            return table;
        } 
    }
}

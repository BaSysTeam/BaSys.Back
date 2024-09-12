using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectTable
    {

        public Guid Uid { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public List<MetaObjectTableColumn> Columns { get; set; } = new List<MetaObjectTableColumn>();

        [IgnoreMember]
        public List<MetaObjectTableColumn> ColumnsWithFormulas => Columns.Where(x=>!string.IsNullOrWhiteSpace(x.Formula)).ToList();

        [IgnoreMember]
        public MetaObjectTableColumn PrimaryKey => Columns.FirstOrDefault(x => x.PrimaryKey);

        public MetaObjectTable Clone()
        {
            var clone = new MetaObjectTable();
            clone.Uid = Uid;
            clone.Title = Title;
            clone.Name = Name;
            clone.Memo = Memo;

            foreach(var source in Columns)
            {
                clone.Columns.Add(source.Clone());
            }


            return clone;
        }

        public void ClearDependencies()
        {
            foreach(var column in Columns)
            {
                column.ClearDependecies();
            }
        }

        public MetaObjectTableColumn GetColumn(string name)
        {
            return Columns.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public MetaObjectTableColumn GetColumn(Guid uid)
        {
            return Columns.FirstOrDefault(x => x.Uid == uid);
        }

        public override string ToString()
        {
            return $"{Title}/{Name}";
        }

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

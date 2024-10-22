using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectCommand
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public Guid TableUid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Expression { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;

        public MetaObjectCommand Clone()
        {
            var clone = new MetaObjectCommand();
            clone.Uid = Uid;
            clone.TableUid = TableUid;
            clone.Title = Title;
            clone.Name = Name;
            clone.Expression = Expression;
            clone.Memo = Memo;

            return clone;
        }

        public override string ToString()
        {
            return $"{Title}/{Name}";
        }
    }
}

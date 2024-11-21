using BaSys.Common.Enums;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectCommand
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public Guid TableUid { get; set; }
        public MetaObjectCommandKinds Kind { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Expression { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public List<MetaObjectCommandParameter> Parameters { get; set; } = new List<MetaObjectCommandParameter>();

        public MetaObjectCommand Clone()
        {
            var clone = new MetaObjectCommand();
            clone.Uid = Uid;
            clone.TableUid = TableUid;
            clone.Kind = Kind;
            clone.Title = Title;
            clone.Name = Name;
            clone.Expression = Expression;
            clone.Memo = Memo;
            clone.IsActive = IsActive;

            foreach (var param in Parameters) { 
                clone.Parameters.Add(param.Clone());
            }


            return clone;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Title)) {
                sb.Append(Title);
            }
            if (!string.IsNullOrEmpty(Name)) { 
                if (sb.Length > 0) { 
                    sb.Append("/"); 
                }
                sb.Append(Name);
            }


            return sb.ToString();
        }
    }
}

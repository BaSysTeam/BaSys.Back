using BaSys.Common.Enums;
using MessagePack;
using System;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class DependencyInfo
    {
        public DependencyKinds Kind { get; set; }
        public Guid FieldUid { get; set; }
        public Guid TableUid { get; set; }

        public DependencyInfo Clone()
        {
            var clone = new DependencyInfo();
            clone.Kind = Kind;
            clone.FieldUid = FieldUid;
            clone.TableUid = TableUid;

            return clone;
        }
    }
}

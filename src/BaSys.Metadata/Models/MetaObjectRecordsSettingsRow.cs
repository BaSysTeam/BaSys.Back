using BaSys.Common.Enums;
using MessagePack;
using System;
using System.Collections.Generic;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectRecordsSettingsRow
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public Guid SourceUid { get; set; }
        public RegisterRecordDirections Direction { get; set; }
        public string Condition { get; set; } = string.Empty;
        public List<MetaObjectRecordsSettingsColumn> Columns { get; set; } = new List<MetaObjectRecordsSettingsColumn>();

        public MetaObjectRecordsSettingsRow Clone() { 

            var clone = new MetaObjectRecordsSettingsRow();
            clone.Uid = Uid;
            clone.SourceUid = SourceUid;
            clone.Direction = Direction;
            clone.Condition = Condition;

            foreach (var column in Columns) {
                clone.Columns.Add(column.Clone());
            }


            return clone;

        }
    }
}

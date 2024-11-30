using MessagePack;
using System;
using System.Collections.Generic;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectRecordsSettingsItem
    {
        /// <summary>
        /// Uid of MetaObject which stores records.
        /// </summary>
        public Guid DestinationMetaObjectUid { get; set; }
        public List<MetaObjectRecordsSettingsRow> Rows { get; set; } = new List<MetaObjectRecordsSettingsRow>();

        public MetaObjectRecordsSettingsItem Clone() { 

            var clone = new MetaObjectRecordsSettingsItem();
            clone.DestinationMetaObjectUid = DestinationMetaObjectUid;
            foreach (var row in Rows) { 
                clone.Rows.Add(row.Clone());
            }

            return clone;

        }
    }
}

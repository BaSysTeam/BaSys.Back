using MessagePack;
using System;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectKindStandardColumn
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public MetaObjectTableColumnDataSettings DataSettings { get; set; } = new MetaObjectTableColumnDataSettings();
        public string Memo { get; set; }

        public MetaObjectKindStandardColumn()
        {
            
        }

        public MetaObjectKindStandardColumn(string uid, string name, string title)
        {
            Uid = Guid.Parse(uid);
            Name = name;
            Title = title;
        }

        public MetaObjectKindStandardColumn(string uid, string name, string title, MetaObjectTableColumnDataSettings dataSettings)
        {
            Uid = Guid.Parse(uid);
            Name = name;
            Title = title;
            DataSettings = dataSettings;
        }

        public MetaObjectKindStandardColumn(Guid uid, string name, string title, Guid dataTypeUid)
        {
            Uid = uid;
            Name = name;
            Title = title;
            DataSettings.DataTypeUid = dataTypeUid;
        }

        public MetaObjectKindStandardColumn(Guid uid,
                                            string name,
                                            string title,
                                            MetaObjectTableColumnDataSettings dataSettings)
        {
            Uid = uid;
            Name = name;
            Title = title;
            DataSettings = dataSettings.Clone();
        }
    }
}

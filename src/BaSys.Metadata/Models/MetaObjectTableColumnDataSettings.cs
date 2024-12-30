using MessagePack;
using System;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed class MetaObjectTableColumnDataSettings
    {
        public Guid DataTypeUid { get; set; }
        public int StringLength { get; set; }
        public int NumberDigits { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Required { get; set; }
        public bool Unique { get; set; }
        public string DefaultValue { get; set; } = String.Empty;

        public MetaObjectTableColumnDataSettings Clone()
        {
            var clone = new MetaObjectTableColumnDataSettings();
            clone.DataTypeUid = DataTypeUid;
            clone.StringLength = StringLength;
            clone.NumberDigits = NumberDigits;
            clone.PrimaryKey = PrimaryKey;
            clone.Required = Required;
            clone.Unique = Unique;
            clone.DefaultValue = DefaultValue;

            return clone;
        }

        public MetaObjectTableColumnDataSettings()
        {
            
        }

        public MetaObjectTableColumnDataSettings(Guid dataTypeUid,
                                                 int stringLength = 0,
                                                 int numberDigits = 0,
                                                 bool required = false,
                                                 bool unique = false,
                                                 bool primaryKey = false)
        {
            DataTypeUid = dataTypeUid;
            StringLength = stringLength;
            NumberDigits = numberDigits;
            Required = required;
            Unique = unique;
            PrimaryKey = primaryKey;
        }

        public override bool Equals(object obj)
        {
            return obj is MetaObjectTableColumnDataSettings settings &&
                   DataTypeUid.Equals(settings.DataTypeUid) &&
                   StringLength == settings.StringLength &&
                   NumberDigits == settings.NumberDigits;
        }

        public bool SettingsEquals(MetaObjectTableColumnDataSettings settings)
        {
            if (settings == null)
                return false;

            return DataTypeUid.Equals(settings.DataTypeUid) &&
                   StringLength == settings.StringLength &&
                   NumberDigits == settings.NumberDigits && 
                   PrimaryKey == settings.PrimaryKey && 
                   Required == settings.Required && 
                   Unique == settings.Unique;
        }

        public bool DataTypeEquals(MetaObjectTableColumnDataSettings settings)
        {
            if (settings == null)
                return false;

            return DataTypeUid.Equals(settings.DataTypeUid) &&
                  StringLength == settings.StringLength &&
                  NumberDigits == settings.NumberDigits;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DataTypeUid, StringLength, NumberDigits);
        }
    }
}

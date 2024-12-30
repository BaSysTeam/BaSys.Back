using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Helpers
{
    public class DataSettingsBuilder
    {
        private MetaObjectTableColumnDataSettings _settings;

        public DataSettingsBuilder()
        {
            _settings = new MetaObjectTableColumnDataSettings();
        }

        public static DataSettingsBuilder Make()
        {
            return new DataSettingsBuilder();
        }

        public DataSettingsBuilder DataType(DataType dataType)
        {
            _settings.DataTypeUid = dataType.Uid;
            return this;
        }

        public DataSettingsBuilder DataType(Guid dataTypeUid)
        {
            _settings.DataTypeUid = dataTypeUid;
            return this;
        }

        public DataSettingsBuilder StringLength(int length)
        {
            _settings.StringLength = length;
            return this;
        }

        public DataSettingsBuilder NumberDigits(int digits)
        {
            _settings.NumberDigits = digits;
            return this;
        }

        public DataSettingsBuilder PrimaryKey(bool isPrimaryKey)
        {
            _settings.PrimaryKey = isPrimaryKey;
            return this;
        }

        public DataSettingsBuilder Required(bool isRequired)
        {
            _settings.Required = isRequired;
            return this;
        }

        public DataSettingsBuilder Unique(bool isUnique)
        {
            _settings.Unique = isUnique;
            return this;
        }

        public DataSettingsBuilder DefaultValue(string defaultValue)
        {
            _settings.DefaultValue = defaultValue;
            return this;
        }

        public MetaObjectTableColumnDataSettings Build()
        {
            return _settings;
        }
    }

}

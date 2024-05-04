using System;
using System.Collections.Generic;
using System.Linq;
using MemoryPack;

namespace BaSys.Metadata.Models
{
    [MemoryPackable]
    public sealed partial class MetaObjectStorableSettings
    {
        public Guid Uid { get; set; }
        public Guid MetaObjectKindUid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public long Version { get; set; }
        public bool IsActive { get; set; }

        [MemoryPackIgnore]
        public List<MetaObjectTable> Tables
        {
            get
            {
                var tables = new List<MetaObjectTable>();
                tables.Add(Header);
                tables.AddRange(TableParts);

                return tables;
            }
        }
        public MetaObjectTable Header { get; set; } = new MetaObjectTable();
        public List<MetaObjectTable> TableParts { get; set; } = new();

        [MemoryPackConstructor]
        public MetaObjectStorableSettings()
        {

        }

        public MetaObjectStorableSettings(MetaObjectKindSettings kindSettings)
        {
            MetaObjectKindUid = kindSettings.Uid;
            Header = MetaObjectTable.HeaderTable();

            var primaryKeySettings = kindSettings.StandardColumns.FirstOrDefault(x => x.IsPrimaryKey);

            if (primaryKeySettings == null)
                throw new ArgumentNullException(nameof(primaryKeySettings), "Primary key is not specified in metaobject kind.");

            var primaryKeyColumn = new MetaObjectTableColumn()
            {
                Title = primaryKeySettings.Title,
                Name = primaryKeySettings.Name,
                DataTypeUid = primaryKeySettings.DataTypeUid,
                NumberDigits = primaryKeySettings.NumberDigits,
                StringLength = primaryKeySettings.StringLength,
                PrimaryKey = true,
                Required = true,
                Unique = true
            };

            Header.Columns.Add(primaryKeyColumn);
        }

        public void CopyFrom(MetaObjectStorableSettings source)
        {
            MetaObjectKindUid = source.Uid;
            Title = source.Title;
            Name = source.Name;
            Memo = source.Memo;
            IsActive = source.IsActive;
            Header = source.Header;
            TableParts = source.TableParts;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}

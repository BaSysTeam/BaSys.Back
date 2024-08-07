﻿using System;
using System.Collections.Generic;
using System.Linq;
using BaSys.Common.Enums;
using MessagePack;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public sealed partial class MetaObjectStorableSettings
    {
        public Guid Uid { get; set; }
        public Guid MetaObjectKindUid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public EditMethods EditMethod { get; set; } = EditMethods.Page;
        public long Version { get; set; }
        public bool IsActive { get; set; }

        [IgnoreMember]
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

        [SerializationConstructor]
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

            // Add primary key column.
            var primaryKeyColumn = new MetaObjectTableColumn()
            {
                Uid = primaryKeySettings.Uid,
                Title = primaryKeySettings.Title,
                Name = primaryKeySettings.Name,
                DataTypeUid = primaryKeySettings.DataTypeUid,
                NumberDigits = primaryKeySettings.NumberDigits,
                StringLength = primaryKeySettings.StringLength,
                PrimaryKey = true,
                Required = true,
                Unique = true,
                IsStandard = true,
            };

            Header.Columns.Add(primaryKeyColumn);

            // Add other standard columns.
            foreach(var stColumn in kindSettings.StandardColumns.Where(x => !x.IsPrimaryKey))
            {
                var newColumn = new MetaObjectTableColumn()
                {
                    Uid = stColumn.Uid,
                    Title= stColumn.Title,
                    Name = stColumn.Name,
                    DataTypeUid = stColumn.DataTypeUid,
                    StringLength= stColumn.StringLength,
                    NumberDigits = stColumn.NumberDigits,
                    PrimaryKey = false,
                    Unique = stColumn.IsUnique,
                    Required = stColumn.IsRequired,
                    IsStandard = true
                };

                Header.Columns.Add(newColumn);
            }

        }

        public void CopyFrom(MetaObjectStorableSettings source)
        {
            MetaObjectKindUid = source.Uid;
            EditMethod = source.EditMethod;
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

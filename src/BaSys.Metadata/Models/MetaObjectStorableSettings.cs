﻿using BaSys.Common.Enums;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public string OrderByExpression { get; set; } = string.Empty;
        public string DisplayExpression { get; set; } = string.Empty;

        public long Version { get; set; }
        public bool IsActive { get; set; }

        [IgnoreMember]
        public List<MetaObjectTable> Tables
        {
            get
            {
                var tables = new List<MetaObjectTable>();
                tables.Add(Header);
                tables.AddRange(DetailTables);

                return tables;
            }
        }
        public MetaObjectTable Header { get; set; } = new MetaObjectTable();
        public List<MetaObjectTable> DetailTables { get; set; } = new();
        public List<MetaObjectCommand> Commands { get; set; } = new();
        public List<MetaObjectRecordsSettingsItem> RecordsSettings { get; set; } = new();

        [SerializationConstructor]
        public MetaObjectStorableSettings()
        {

        }

        public MetaObjectStorableSettings(MetaObjectKindSettings kindSettings)
        {
            MetaObjectKindUid = kindSettings.Uid;
            Header = MetaObjectTable.HeaderTable();

            var primaryKeySettings = kindSettings.StandardColumns.FirstOrDefault(x => x.DataSettings.PrimaryKey);

            if (primaryKeySettings == null)
                throw new ArgumentNullException(nameof(primaryKeySettings), "Primary key is not specified in metaobject kind.");

            // Add primary key column.
            var primaryKeyColumn = new MetaObjectTableColumn()
            {
                Uid = primaryKeySettings.Uid,
                Title = primaryKeySettings.Title,
                Name = primaryKeySettings.Name,
                IsStandard = true,
            };

            primaryKeyColumn.DataSettings.DataTypeUid = primaryKeySettings.DataSettings.DataTypeUid;
            primaryKeyColumn.DataSettings.NumberDigits = primaryKeySettings.DataSettings.NumberDigits;
            primaryKeyColumn.DataSettings.StringLength = primaryKeySettings.DataSettings.StringLength;
            primaryKeyColumn.DataSettings.PrimaryKey = true;
            primaryKeyColumn.DataSettings.Required = true;
            primaryKeyColumn.DataSettings.Unique = true;

            Header.Columns.Add(primaryKeyColumn);

            // Add other standard columns.
            foreach (var stColumn in kindSettings.StandardColumns.Where(x => !x.DataSettings.PrimaryKey))
            {
                var newColumn = new MetaObjectTableColumn()
                {
                    Uid = stColumn.Uid,
                    Title = stColumn.Title,
                    Name = stColumn.Name,
                    IsStandard = true
                };

                newColumn.DataSettings.DataTypeUid = stColumn.DataSettings.DataTypeUid;
                newColumn.DataSettings.StringLength = stColumn.DataSettings.StringLength;
                newColumn.DataSettings.NumberDigits = stColumn.DataSettings.NumberDigits;
                newColumn.DataSettings.PrimaryKey = false;
                newColumn.DataSettings.Unique = stColumn.DataSettings.Unique;
                newColumn.DataSettings.Required = stColumn.DataSettings.Required;

                Header.Columns.Add(newColumn);
            }

        }

        public void CopyFrom(MetaObjectStorableSettings source)
        {
            MetaObjectKindUid = source.MetaObjectKindUid;
            EditMethod = source.EditMethod;
            Title = source.Title;
            Name = source.Name;
            Memo = source.Memo;
            OrderByExpression = source.OrderByExpression;
            DisplayExpression = source.DisplayExpression;
            IsActive = source.IsActive;
            Header = source.Header;
            DetailTables = source.DetailTables;
            Commands = source.Commands;
            RecordsSettings = source.RecordsSettings;
        }

        public MetaObjectStorableSettings Clone()
        {
            var clone = new MetaObjectStorableSettings();
            clone.Uid = Uid;
            clone.MetaObjectKindUid = MetaObjectKindUid;
            clone.EditMethod = EditMethod;
            clone.Title = Title;
            clone.Name = Name;
            clone.Memo = Memo;
            clone.OrderByExpression = OrderByExpression;
            clone.DisplayExpression = DisplayExpression;
            clone.IsActive = IsActive;

            clone.Header = Header.Clone();

            foreach (var item in DetailTables)
            {
                clone.DetailTables.Add(item.Clone());
            }

            foreach (var item in Commands) { 
                clone.Commands.Add(item.Clone());
            }

            foreach (var item in RecordsSettings)
            {
                clone.RecordsSettings.Add(item.Clone());
            }

            return clone;
        }

        public void ClearDependencies()
        {
            foreach (var table in Tables)
            {
                table.ClearDependencies();
            }
        }

        public string GetOrderByExpression(string defaultExpression)
        {
            if (string.IsNullOrWhiteSpace(OrderByExpression))
            {
                return defaultExpression;
            }
            else
            {
                return OrderByExpression;
            }
        }

        public string GetDisplayExpression(string defaultExpression, string pkName)
        {
            var result = defaultExpression;
            if (!string.IsNullOrWhiteSpace(DisplayExpression))
            {
                result = DisplayExpression;
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                result = pkName;
            }

            return result;
        }

        public MetaObjectTable GetTable(string tableName)
        {
            return this.DetailTables.FirstOrDefault(x=>x.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
        }

        public override string ToString()
        {
            return $"{Title}/{Name}";
        }
    }
}

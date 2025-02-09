﻿using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Models;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;

namespace BaSys.Host.DAL.ModelConfigurations
{
    public sealed class DataObjectConfiguration : DataModelConfiguration<DataObject>
    {

        private readonly MetaObjectKindSettings _kindSettings;
        private readonly MetaObjectStorableSettings _objectSettings;
        private readonly IDataTypesIndex _dataTypesIndex;

        public DataObjectConfiguration(MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            IDataTypesIndex dataTypesIndex) : base(false)
        {

            _kindSettings = kindSettings;
            _objectSettings = objectSettings;
            _dataTypesIndex = dataTypesIndex;

            Table(ComposeTableName(_kindSettings.Prefix, _objectSettings.Name));

            ClearColumns();

            AddPrimaryKey();
            AddColumns();

        }

        public static string ComposeTableName(string kindPrefix, string objectName)
        {
            return $"{kindPrefix}_{objectName}";
        }

        private void AddPrimaryKey()
        {
            // Add primary key.
            var pkSettings = _objectSettings.Header.PrimaryKey;

            if (pkSettings == null)
                throw new ArgumentNullException($"Primary key is not spicified for {_kindSettings.Title}.{_objectSettings.Title}");

            var tableColumn = new TableColumn()
            {
                Name = pkSettings.Name,
                PrimaryKey = true,
                DbType = _dataTypesIndex.GetDbType(pkSettings.DataSettings.DataTypeUid),
                StringLength = pkSettings.DataSettings.StringLength,
                NumberDigits = pkSettings.DataSettings.NumberDigits,
            };
            AddColumn(tableColumn);
        }

        private void AddColumns()
        {
            // Add other columns.
            foreach (var columnSettings in _objectSettings.Header.Columns.Where(x => !x.DataSettings.PrimaryKey))
            {
                var tableColumn = new TableColumn()
                {
                    Name = columnSettings.Name,
                    PrimaryKey = false,
                    DbType = _dataTypesIndex.GetDbType(columnSettings.DataSettings.DataTypeUid),
                    StringLength = columnSettings.DataSettings.StringLength,
                    NumberDigits = columnSettings.DataSettings.NumberDigits,
                    Required = columnSettings.DataSettings.Required,
                    Unique = columnSettings.DataSettings.Unique,
                };

                AddColumn(tableColumn);

            }
        }
    }
}

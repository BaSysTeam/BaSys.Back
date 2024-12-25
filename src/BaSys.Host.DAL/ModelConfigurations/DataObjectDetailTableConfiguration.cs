using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Models;
using BaSys.Metadata.Abstractions;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.ModelConfigurations
{
    public sealed class DataObjectDetailTableConfiguration: DataModelConfiguration<DataObjectDetailsTable>
    {
        private readonly MetaObjectKindSettings _kindSettings;
        private readonly MetaObjectStorableSettings _objectSettings;
        private readonly MetaObjectTable _tableSettings;
        private readonly IDataTypesIndex _dataTypesIndex;

        public DataObjectDetailTableConfiguration(MetaObjectKindSettings kindSettings,
            MetaObjectStorableSettings objectSettings,
            MetaObjectTable tableSettings,
            IDataTypesIndex dataTypesIndex): base(false)
        {
            _kindSettings = kindSettings;
            _objectSettings = objectSettings;
            _tableSettings = tableSettings;
            _dataTypesIndex = dataTypesIndex;

            Table(ComposeTableName(_kindSettings.Prefix, _objectSettings.Name, _tableSettings.Name));
            AddColumns();
        }

        public static string ComposeTableName(string kindPrefix, string objectName, string tableName)
        {
            return $"{kindPrefix}_{objectName}_dt_{tableName}";
        }

        private void AddColumns()
        {
            // Add other columns.
            foreach (var columnSettings in _tableSettings.Columns)
            {
                var tableColumn = new TableColumn()
                {
                    Name = columnSettings.Name,
                    PrimaryKey = columnSettings.DataSettings.PrimaryKey,
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

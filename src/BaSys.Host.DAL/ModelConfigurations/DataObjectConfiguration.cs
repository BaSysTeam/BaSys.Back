using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Models;
using BaSys.Metadata.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.ModelConfigurations
{
    public sealed class DataObjectConfiguration: DataModelConfiguration<DataObject>
    {

        private readonly MetaObjectKindSettings _kindSettings;
        private readonly MetaObjectStorableSettings _objectSettings;
        private readonly PrimitiveDataTypes _primitiveDataTypes;

        public DataObjectConfiguration(MetaObjectKindSettings kindSettings, 
            MetaObjectStorableSettings objectSettings, 
            PrimitiveDataTypes primitiveDataTypes):base(false)
        {

            _kindSettings = kindSettings;
            _objectSettings = objectSettings;
            _primitiveDataTypes = primitiveDataTypes;

            Table($"{kindSettings.Prefix}_{objectSettings.Name}");

            ClearColumns();

            AddPrimaryKey();
            AddColumns();
           
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
                DbType = _primitiveDataTypes.GetDbType(pkSettings.DataTypeUid)
            };
            AddColumn( tableColumn );
        }

        private void AddColumns()
        {
            // Add other columns.
            foreach (var columnSettings in _objectSettings.Header.Columns.Where(x => !x.PrimaryKey))
            {
                var tableColumn = new TableColumn()
                {
                    Name = columnSettings.Name,
                    PrimaryKey = false,
                    DbType = _primitiveDataTypes.GetDbType(columnSettings.DataTypeUid),
                    StringLength = columnSettings.StringLength,
                    NumberDigits = columnSettings.NumberDigits,
                    Required = columnSettings.Required,
                    Unique = columnSettings.Unique,
                };

                AddColumn( tableColumn );

            }
        }
    }
}

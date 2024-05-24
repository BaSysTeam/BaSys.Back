using BaSys.DAL.Models.App;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BaSys.DTO.App
{
    public sealed class DataObjectWithMetadataDto
    {
        public MetaObjectKindSettings MetaObjectKindSettings { get; set; } = new MetaObjectKindSettings();
        public MetaObjectStorableSettings MetaObjectSettings { get; set; } = new MetaObjectStorableSettings();
        public DataObjectDto Item { get; set; } = new DataObjectDto();

        public DataObjectWithMetadataDto()
        {
            
        }

        public DataObjectWithMetadataDto(MetaObjectKindSettings kindSettings, MetaObjectStorableSettings objectSettings, DataObject dataObject)
        {
            MetaObjectKindSettings = kindSettings;
            MetaObjectSettings = objectSettings;

            Item = new DataObjectDto(dataObject);
        }
    }
}

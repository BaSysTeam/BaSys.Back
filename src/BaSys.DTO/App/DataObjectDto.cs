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
    public sealed class DataObjectDto
    {
        public MetaObjectKindSettings MetaObjectKindSettings { get; set; } = new MetaObjectKindSettings();
        public MetaObjectStorableSettings MetaObjectSettings { get; set; } = new MetaObjectStorableSettings();
        public Dictionary<string, object> Header { get; set; } = new Dictionary<string, object>();

        public DataObjectDto()
        {
            
        }

        public DataObjectDto(MetaObjectKindSettings kindSettings, MetaObjectStorableSettings objectSettings, DataObject dataObject)
        {
            MetaObjectKindSettings = kindSettings;
            MetaObjectSettings = objectSettings;

            foreach (var kvp in dataObject.Header)
            {
                Header[kvp.Key] = dataObject.Header[kvp.Key];
            }
        }
    }
}

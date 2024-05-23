using BaSys.DAL.Models.App;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.App
{
    public sealed class DataObjectListDto
    {
        public MetaObjectKindSettings MetaObjectKindSettings { get; set; } = new MetaObjectKindSettings();
        public MetaObjectStorableSettings MetaObjectSettings { get; set; } = new MetaObjectStorableSettings();  

        public List<DataObjectSaveDto> Items { get; set; } = new List<DataObjectSaveDto>();

        public DataObjectListDto()
        {
            
        }

        public DataObjectListDto(MetaObjectKindSettings kindSettings, MetaObjectStorableSettings objectSettings, IEnumerable<DataObject> items)
        {
            MetaObjectKindSettings = kindSettings;
            MetaObjectSettings = objectSettings;

            foreach (var dataItem in items)
            {
                var itemDto = new DataObjectSaveDto(kindSettings.Uid, objectSettings.Uid, dataItem.Header);
               Items.Add(itemDto);
            }

        }

    }
}

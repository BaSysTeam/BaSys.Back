﻿using BaSys.DAL.Models.App;
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

        public List<DataObjectDto> Items { get; set; } = new List<DataObjectDto>();

        public DataObjectListDto()
        {
            
        }

        public DataObjectListDto(MetaObjectKindSettings kindSettings, MetaObjectStorableSettings objectSettings, IEnumerable<DataObject> items)
        {
            MetaObjectKindSettings = kindSettings;
            MetaObjectSettings = objectSettings;

            foreach (var dataItem in items)
            {
                var itemDto = new DataObjectDto(dataItem);
               Items.Add(itemDto);
            }

        }

    }
}

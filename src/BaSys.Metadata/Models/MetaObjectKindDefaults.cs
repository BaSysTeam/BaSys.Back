using BaSys.Translation;
using System;
using System.Collections.Generic;

namespace BaSys.Metadata.Models
{
    public static class MetaObjectKindDefaults
    {
        public static readonly MetaObjectKindSettings Menu = new MetaObjectKindSettings()
        {
            Uid = Guid.Parse("{4FB2CC15-2579-4D01-AEDB-F097430756F7}"),
            Name = "menu",
            Title = DictMain.Menu,
            Prefix = "mnu",
            IconClass = "pi pi-list",
            IsStandard = true
        };

        public static readonly MetaObjectKindSettings Catalog = BuildDefaultCatalogKind();

        public static IEnumerable<MetaObjectKindSettings> AllItems()
        {
            var collection = new List<MetaObjectKindSettings>
            {
                Menu,
                Catalog,
            };

            return collection;
        }

        private static MetaObjectKindSettings BuildDefaultCatalogKind()
        {
            var settings = new MetaObjectKindSettings()
            {
                Uid = Guid.Parse("{032D8377-500F-4631-B435-1F7F69046674}"),
                Name = "catalog",
                Prefix = "cat",
                Title = DictMain.Catalog,
                IconClass = "pi pi-book",
                StoreData = true,
                IsReference = true,
                AllowAttacheFiles = true,
                OrderByExpression = "title",
                DisplayExpression = "title",
            };

            var idColumn = new MetaObjectKindStandardColumn(Guid.Parse("{4A7C739C-6012-4E45-BFC0-B78A06DB8AEC}"),
                                                            "id",
                                                            "Id",
                                                            new MetaObjectTableColumnDataSettings(dataTypeUid: DataTypeDefaults.Int.Uid,
                                                                                                  primaryKey: true));

            var idTitle = new MetaObjectKindStandardColumn(Guid.Parse("{E163C0C4-DE77-4E58-A468-F3527B573E4A}"),
                                                          "title",
                                                           DictMain.Title,
                                                           new MetaObjectTableColumnDataSettings(dataTypeUid: DataTypeDefaults.String.Uid,
                                                                                                 stringLength: 100,
                                                                                                 required: true) );

            settings.StandardColumns.Add(idColumn);
            settings.StandardColumns.Add(idTitle);

            return settings;
        }
    }
}

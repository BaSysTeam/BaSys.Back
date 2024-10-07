using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    public static class MetaObjectKindDefaults
    {
        public static readonly MetaObjectKindSettings Menu = new MetaObjectKindSettings()
        {
            Uid = Guid.Parse("{4FB2CC15-2579-4D01-AEDB-F097430756F7}"),
            Name = "menu",
            Title = "Menu",
            Prefix = "mnu",
            IconClass = "pi pi-list",
            IsStandard = true
        };

        public static IEnumerable<MetaObjectKindSettings> AllItems()
        {
            var collection = new List<MetaObjectKindSettings>();
            collection.Add(Menu);

            return collection;
        }
    }
}

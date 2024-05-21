using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models
{
    public class RightRecord
    {
        public Guid Uid { get; set; }
        public Guid UserGroupUid { get; set; }
        public Guid ApplicationRightUid { get; set; }
        public Guid? MetaObjectKindUid { get; set; }
        public Guid? MetaObjectUid { get; set; }
        public bool IsChecked { get; set; }
    }
}

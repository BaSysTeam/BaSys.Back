using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Core.Features.RecordsBuilder
{
    public sealed class RecordSettingsRequiredColumns
    {
        public MetaObjectTableColumn MetaObjectKindColumn { get; set; }
        public MetaObjectTableColumn MetaObjectColumn { get; set; }
        public MetaObjectTableColumn ObjectColumn { get; set; }
        public MetaObjectTableColumn RowColumn { get; set; }
    }
}

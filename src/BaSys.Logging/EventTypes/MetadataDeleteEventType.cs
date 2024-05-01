using BaSys.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Logging.EventTypes
{
    public sealed class MetadataDeleteEventType : EventType
    {
        public override Guid Uid => new Guid("{B8438C5A-460F-428B-A11E-006A862818C9}");
        public override string EventName => "MetadataDelete";
    }
}

using BaSys.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Logging.EventTypes
{
    public sealed class MetadataUpdateEventType : EventType
    {
        public override Guid Uid => new Guid("{C8B24AAA-0EA1-4A17-B279-9A7B7EE65F58}");
        public override string EventName => "MetadataUpdate";
    }
}

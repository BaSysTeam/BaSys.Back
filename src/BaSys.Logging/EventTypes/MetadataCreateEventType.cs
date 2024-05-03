using BaSys.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Logging.EventTypes
{
    public sealed class MetadataCreateEventType : EventType
    {
        public override Guid Uid => new Guid("{7B8A9352-1CA1-4938-BD03-4C9D368905E4}");
        public override string EventName => "MetadataCreate";
    }
}

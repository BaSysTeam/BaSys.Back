using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Logging.EventTypes
{
    public static class EventTypeFactory
    {
        public static MetadataCreateEventType MetadataCreate => new MetadataCreateEventType();
        public static MetadataUpdateEventType MetadataUpdate => new MetadataUpdateEventType();  
        public static MetadataDeleteEventType MetadataDelete => new MetadataDeleteEventType();  
        public static SettingsChangedEventType SettingsChanged => new SettingsChangedEventType();   
        public static UserLoginEventType UserLogin => new UserLoginEventType();

    }
}

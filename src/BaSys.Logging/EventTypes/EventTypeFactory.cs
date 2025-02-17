namespace BaSys.Logging.EventTypes
{
    public static class EventTypeFactory
    {
        public static DataObjectCreateEventType DataObjectCreate => new DataObjectCreateEventType();
        public static DataObjectUpdateEventType DataObjectUpdate => new DataObjectUpdateEventType();
        public static DataObjectDeleteEventType DataObjectDelete => new DataObjectDeleteEventType();
        public static MetadataCreateEventType MetadataCreate => new MetadataCreateEventType();
        public static MetadataUpdateEventType MetadataUpdate => new MetadataUpdateEventType();  
        public static MetadataDeleteEventType MetadataDelete => new MetadataDeleteEventType();  
        public static SettingsChangedEventType SettingsChanged => new SettingsChangedEventType();
        public static TriggerStartEventType TriggerStart => new TriggerStartEventType();
        public static UserLoginEventType UserLogin => new UserLoginEventType();
        public static UserLoginFailEventType UserLoginFail => new UserLoginFailEventType();
        public static PublicApiEventType PublicApi => new PublicApiEventType();
    }
}

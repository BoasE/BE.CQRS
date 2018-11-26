namespace BE.CQRS.Domain.Events
{
    public static class EventHeaderKeys
    {
        public const string AggregateType = "AggregateType";
        public const string AggregateId = "AggregateId";
        public const string CommitId = "CommitId";
        public const string Created = "Created";
        public const string EventId = "EventId";
        public const string UserId = "UserId";
        public const string EventType = "EventType";
        public const string AssemblyEventType = "AssemblyEventType";
        public const string EventNumber = "EventNumber";
        public const string Timestamp = "Timestamp";
    }
}
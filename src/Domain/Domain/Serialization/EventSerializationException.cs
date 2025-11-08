using System;

namespace BE.CQRS.Domain.Serialization
{
    /// <summary>
    /// Exception that is thrown when an event cannot be serialized or deserialized.
    /// </summary>
    public class EventSerializationException : Exception
    {
        public string EventData { get; }
        public string EventType { get; }
        public string AggregateId { get; }

        public EventSerializationException(string message) : base(message)
        {
        }

        public EventSerializationException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public EventSerializationException(string message, string eventType, string aggregateId,
            Exception? innerException)
            : base(message, innerException)
        {
            EventType = eventType;
            AggregateId = aggregateId;
        }

        public EventSerializationException(string message, string eventType, string aggregateId, string eventData,
            Exception? innerException)
            : base(message, innerException)
        {
            EventType = eventType;
            AggregateId = aggregateId;
            EventData = eventData;
        }
    }
}
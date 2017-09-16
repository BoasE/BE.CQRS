using System;

namespace BE.CQRS.Domain.Events
{
    public abstract class EventBase : IEvent
    {
        public EventHeader Headers { get; } = new EventHeader();

        protected EventBase()
        {
            Type type = GetType();
            Headers.Set(EventHeaderKeys.EventType, type.Name);
            Headers.Set(EventHeaderKeys.AssemblyEventType, GetType().AssemblyQualifiedName);
            Headers.Set(EventHeaderKeys.EventId, Guid.NewGuid());
        }

        protected EventBase(EventHeader headers) : this()
        {
            Headers.ApplyEventHeader(headers);
        }
    }
}
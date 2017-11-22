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

        public virtual bool Validate() => true;

        public void AssertValidation()
        {
            if(!Validate())
            {
                throw new InvalidOperationException("Event is in a invalid state !"); //TODO Better exception type and more information what is wrong
            }
        }
    }
}
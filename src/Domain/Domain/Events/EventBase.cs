using System;
using System.Text.Json.Serialization;

namespace BE.CQRS.Domain.Events
{
    public abstract class EventBase : IEvent
    {
        [JsonIgnore]
        public EventHeader Headers { get; } = new EventHeader();

        protected EventBase()
        {
            Type type = GetType();
            Headers.Set(EventHeaderKeys.EventType, type.Name);
            Headers.Set(EventHeaderKeys.AssemblyEventType, GetType().AssemblyQualifiedName);
            Headers.Set(EventHeaderKeys.EventId, Guid.NewGuid());
            Headers.Set(EventHeaderKeys.EventFrameworkVersion,CurrentVersion.FrameworkEventVersion.Value);
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
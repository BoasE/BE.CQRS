using System;
using System.Collections.Generic;
using System.Text.Json;
using BE.CQRS.Domain.Events;
using BE.FluentGuard;

namespace BE.CQRS.Domain.Serialization
{
    public class JsonEventSerializer : IEventSerializer
    {
        private readonly IEventTypeResolver eventTypeResolver;

        private readonly JsonSerializerOptions options = new ()
        {
        };

        public JsonEventSerializer(IEventTypeResolver eventTypeResolver)
        {
            this.eventTypeResolver = eventTypeResolver;
        }

        protected virtual EventHeader DeserializeHeader(string metaData)
        {
            Dictionary<string, string> result = JsonSerializer.Deserialize<Dictionary<string, string>>(metaData);

            return new EventHeader(result);
        }

        public virtual IEvent DeserializeEvent(Dictionary<string, string> headerData, string eventData)
        {
            var header = new EventHeader(headerData);
            Type type = eventTypeResolver.ResolveType(header);

            IEvent result = DeserializeEventInternal(eventData, type);

            result?.Headers.ApplyEventHeader(header);

            return result;
        }

        public virtual IEvent DeserializeEvent(string headerData, string eventData)
        {
            EventHeader header = DeserializeHeader(headerData);

            Type type = eventTypeResolver.ResolveType(header);
            IEvent result = DeserializeEventInternal(eventData, type);
            result?.Headers.ApplyEventHeader(header);

            return result;
        }

        public virtual string SerializeHeader(EventHeader headers)
        {
            Precondition.For(headers, nameof(headers)).NotNull();
            return JsonSerializer.Serialize(headers.ToDictionary());
        }

        public virtual string SerializeEvent(IEvent @event)
        {
            Precondition.For(@event, nameof(@event)).NotNull();
            return SerializeEventInternal(@event);
        }

        protected virtual string SerializeEventInternal(IEvent @event)
        {
            return JsonSerializer.Serialize(@event, @event.GetType());
        }

        protected virtual IEvent DeserializeEventInternal(string eventData, Type type)
        {
            return JsonSerializer.Deserialize(eventData, type) as IEvent;
        }
    }
}
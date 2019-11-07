using System;
using System.Collections.Generic;
using System.Text.Json;
using BE.CQRS.Domain.Events;
using BE.FluentGuard;

namespace BE.CQRS.Domain.Serialization
{
    public sealed class JsonEventSerializer : IEventSerializer
    {
        private readonly IEventTypeResolver eventTypeResolver;

        private readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
        };

        public JsonEventSerializer(IEventTypeResolver eventTypeResolver)
        {
            this.eventTypeResolver = eventTypeResolver;
        }

        private EventHeader DeserializeHeader(string metaData)
        {
            Dictionary<string, string> result = JsonSerializer.Deserialize<Dictionary<string, string>>(metaData);

            return new EventHeader(result);
        }

        public IEvent DeserializeEvent(Dictionary<string, string> headerData, string eventData)
        {
            var header = new EventHeader(headerData);
            Type type = eventTypeResolver.ResolveType(header);

            var result = JsonSerializer.Deserialize(eventData, type) as IEvent;

            result?.Headers.ApplyEventHeader(header);

            return result;
        }

        public IEvent DeserializeEvent(string headerData, string eventData)
        {
            EventHeader header = DeserializeHeader(headerData);

            Type type = eventTypeResolver.ResolveType(header);
            var result = JsonSerializer.Deserialize(eventData, type) as IEvent;
            result?.Headers.ApplyEventHeader(header);

            return result;
        }

        public string SerializeHeader(EventHeader headers)
        {
            Precondition.For(headers, nameof(headers)).NotNull();
            string result = JsonSerializer.Serialize(headers.ToDictionary());
            return result;
        }

        public string SerializeEvent(IEvent @event)
        {
            Precondition.For(@event, nameof(@event)).NotNull();

            string result = JsonSerializer.Serialize(@event, @event.GetType());
            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using BE.CQRS.Domain.Events;
using BE.FluentGuard;
using Newtonsoft.Json;

namespace BE.CQRS.Domain.Serialization
{
    public sealed class JsonEventSerializer : IEventSerializer
    {
        private readonly IEventTypeResolver eventTypeResolver;

        internal static JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings
        {
            ContractResolver = new EventContractResolver()
        };

        public JsonEventSerializer(IEventTypeResolver eventTypeResolver)
        {
            this.eventTypeResolver = eventTypeResolver;
        }

        private EventHeader DeserializeHeader(string metaData)
        {
            var vals = JsonConvert.DeserializeObject<Dictionary<string, string>>(metaData, SerializerSettings);
            return new EventHeader(vals);
        }

        public IEvent DeserializeEvent(Dictionary<string, string> headerData, string eventData)
        {
            var header = new EventHeader(headerData);
            Type type = eventTypeResolver.ResolveType(header);

            var result = JsonConvert.DeserializeObject(eventData, type, SerializerSettings) as IEvent;

            result?.Headers.ApplyEventHeader(header);

            return result;
        }

        public IEvent DeserializeEvent(string headerData, string eventData)
        {
            EventHeader header = DeserializeHeader(headerData);

            Type type = eventTypeResolver.ResolveType(header);

            var result = JsonConvert.DeserializeObject(eventData, type, SerializerSettings) as IEvent;

            result?.Headers.ApplyEventHeader(header);

            return result;
        }

        public string SerializeHeader(EventHeader headers)
        {
            Precondition.For(headers, nameof(headers)).NotNull();

            return JsonConvert.SerializeObject(headers.ToDictionary(), SerializerSettings);
        }

        public string SerializeEvent(IEvent @event)
        {
            Precondition.For(@event, nameof(@event)).NotNull();

            return JsonConvert.SerializeObject(@event, SerializerSettings);
        }
    }
}
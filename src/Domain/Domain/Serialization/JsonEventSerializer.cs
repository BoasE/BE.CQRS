using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using BE.CQRS.Domain.Events;
using BE.FluentGuard;

namespace BE.CQRS.Domain.Serialization
{
    public class JsonEventSerializer : IEventSerializer
    {
        private readonly IEventTypeResolver eventTypeResolver;

        private readonly JsonSerializerOptions options = new()
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
            var result = DeserializeEvent(eventData, type, header);

            return result;
        }


        public virtual IEvent DeserializeEvent(string headerData, string eventData)
        {
            Precondition.For(headerData, nameof(headerData)).NotNullOrWhiteSpace();
            Precondition.For(eventData, nameof(eventData)).NotNullOrWhiteSpace();

            EventHeader header = DeserializeHeader(headerData);

            Type type = eventTypeResolver.ResolveType(header);
            var result = DeserializeEvent(eventData, type, header);

            return result;
        }

        private IEvent DeserializeEvent(string eventData, Type type, EventHeader header)
        {
            AssertType(type, header);
            IEvent result;
            try
            {
                result = DeserializeEventInternal(eventData, type);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Event {type} for {header.AggregateId} could not be deserialized", e);
            }


            return result;
        }

        private static void AssertType([NotNull] Type type, EventHeader header)
        {
            if (string.IsNullOrWhiteSpace(header.AssemblyEventType))
            {
                throw new InvalidOperationException("EventHeader must have a AssemblyEventType set");
            }

            if (type == null)
            {
                throw new InvalidOperationException($"Type \"{header.AssemblyEventType}\" could not be resolved");
            }
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
            Precondition.For(type, nameof(type)).NotNull();
            Precondition.For(eventData, nameof(eventData)).NotNullOrWhiteSpace();
            return JsonSerializer.Deserialize(eventData, type) as IEvent;
        }
    }
}
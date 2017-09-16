using System;
using System.Reflection;
using BE.CQRS.Domain.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BE.CQRS.Domain.Serialization
{
    public class EventContractResolver : DefaultContractResolver
    {
        private static readonly Type EventHeaderType = typeof(EventHeader);
        private static readonly Type TypeType = typeof(Type);

        public static readonly EventContractResolver Instance =
            new EventContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member,
            MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            bool preventSerialize = property.Ignored || property.PropertyType == EventHeaderType ||
                property.PropertyType == TypeType;

            property.ShouldSerialize = instance => !preventSerialize;

            return property;
        }
    }
}
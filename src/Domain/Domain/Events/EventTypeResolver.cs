using System;

namespace BE.CQRS.Domain.Events
{
    public sealed class EventTypeResolver : IEventTypeResolver
    {
        public Type ResolveType(EventHeader header)
        {
            Type type = Type.GetType(header.AssemblyEventType);

            return type;
        }
    }
}
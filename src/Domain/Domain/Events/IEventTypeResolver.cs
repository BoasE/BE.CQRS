using System;

namespace BE.CQRS.Domain.Events
{
    public interface IEventTypeResolver
    {
        Type ResolveType(EventHeader header);
    }
}
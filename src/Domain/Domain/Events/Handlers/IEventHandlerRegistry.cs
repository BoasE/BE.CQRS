using System;
using System.Collections.Generic;

namespace BE.CQRS.Domain.Events.Handlers
{
    public interface IEventHandlerRegistry
    {
        EventHandlerMethod[] Resolve(Type eventType);

        void Add(IEnumerable<EventHandlerMethod> methods);
    }
}
using System;
using System.Collections.Generic;

namespace BE.CQRS.Domain.Events.Handlers
{
    public interface IEventMethodConvetion
    {
        IEnumerable<EventHandlerMethod> ResolveEventMethods(Type type);
    }
}
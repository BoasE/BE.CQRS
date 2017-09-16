using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BE.CQRS.Domain.Events.Handlers
{
    public sealed class EventHandlerRegistry : IEventHandlerRegistry
    {
        private readonly Dictionary<TypeInfo, List<EventHandlerMethod>> knownEventMethods =
            new Dictionary<TypeInfo, List<EventHandlerMethod>>();

        private readonly ConcurrentDictionary<TypeInfo, ICollection<EventHandlerMethod>> resolvedMappings =
            new ConcurrentDictionary<TypeInfo, ICollection<EventHandlerMethod>>();

        public void Add(IEnumerable<EventHandlerMethod> methods)
        {
            foreach (EventHandlerMethod method in methods)
            {
                if (!knownEventMethods.ContainsKey(method.EventTypeInfo))
                {
                    knownEventMethods.Add(method.EventTypeInfo, new List<EventHandlerMethod>());
                }
                knownEventMethods[method.EventTypeInfo].Add(method);
            }
        }

        public EventHandlerMethod[] Resolve(Type eventType)
        {
            TypeInfo info = eventType.GetTypeInfo();
            ICollection<EventHandlerMethod> result = resolvedMappings.GetOrAdd(info, FindMatchingEvents);

            return result.ToArray();
        }

        private ICollection<EventHandlerMethod> FindMatchingEvents(TypeInfo eventType)
        {
            return knownEventMethods.Where(x => x.Key.IsAssignableFrom(eventType)).SelectMany(x => x.Value).ToArray();
        }
    }
}
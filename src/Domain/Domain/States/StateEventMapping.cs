using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Events.Handlers;

namespace BE.CQRS.Domain.States
{
    public interface IStateEventMapping
    {
        ISet<Type> ResolveRequiredEvents(IState state);
    }

    public class StateEventMapping : IStateEventMapping
    {
        private readonly ConcurrentDictionary<Type, ISet<Type>> cache = new ConcurrentDictionary<Type, ISet<Type>>();

        private readonly IEventMethodConvetion methodConvention = new OnPrefixEventMethodConvetion();

        public StateEventMapping()
        {
        }

        public ISet<Type> ResolveRequiredEvents(IState state)
        {
            return cache.GetOrAdd(state.GetType(), InternalResolve);
        }

        private ISet<Type> InternalResolve(Type state)
        {
            var eventType = typeof(IEvent);
            IEnumerable<EventHandlerMethod> eventMethods = methodConvention.ResolveEventMethods(state);

            HashSet<Type> eventTypes = new HashSet<Type>();
            foreach (var eventMethod in eventMethods)
            {
                var eventParemeters = eventMethod.Method.GetParameters().Where(x => eventType.IsAssignableFrom(x.ParameterType));

                foreach (ParameterInfo eventParameter in eventParemeters)
                {
                    eventTypes.Add(eventParameter.ParameterType);
                }
            }

            return eventTypes;
        }
    }
}
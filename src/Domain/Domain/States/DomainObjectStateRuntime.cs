using System;
using System.Collections.Generic;
using System.Linq;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Policies;

namespace BE.CQRS.Domain.States
{
    public sealed class DomainObjectStateRuntime
    {
        private readonly IStateEventMapping stateToEventMapper;
        private readonly IStateActivator activator;
        private readonly IDomainObject domainObject;

        public DomainObjectStateRuntime(IDomainObject domainObject,IStateActivator stateActivator, EventSourceConfiguration config)
        {
            this.domainObject = domainObject;
            activator = stateActivator;
            stateToEventMapper = config.StateToEventMapper;
        }

        public bool Policy<T>(bool includeUncommitted) where T : PolicyBase, new()
        {
            var state = new T();

            ExecuteState(includeUncommitted, state);

            return state.IsValid();
        }

        public bool Policy<T>(ICommand command, bool includeUncommitted) where T : PolicyBase
        {
            return Policy(typeof(T), command, includeUncommitted);
        }

        private readonly Type commandPolicyType = typeof(CommandPolicyBase<>);

        public bool Policy(Type policy, ICommand command, bool includeUncommitted)
        {
            PolicyBase state;
            if (commandPolicyType.IsAssignableFrom(policy))
                state = Activator.CreateInstance(policy, command) as PolicyBase;
            else
                state = Activator.CreateInstance(policy) as PolicyBase;

            if (state == null)
                throw new InvalidOperationException();

            ExecuteState(includeUncommitted, state);

            return state.IsValid();
        }

        public T State<T>(bool excludeUncommitted) where T : class, IState
        {
            return StateInternal<T>(excludeUncommitted);
        }

        private T StateInternal<T>(bool excludeUncommitted) where T : class, IState
        {
            var state = activator.ResolveState<T>();

            ExecuteState(excludeUncommitted, state);

            return state;
        }

        private void ExecuteState(bool excludeUncommitted, IState state)
        {
            var events = new List<IEvent>(domainObject.GetCommittedEvents());

            if (!excludeUncommitted)
                events.AddRange(domainObject.GetUncommittedEvents());

            state.Execute(FilterForRequiredEvents(state, events));
        }

        private IEnumerable<IEvent> FilterForRequiredEvents(IState state, IEnumerable<IEvent> events)
        {
            ISet<Type> requiredEventTypes = stateToEventMapper.ResolveRequiredEvents(state);

            if (requiredEventTypes == null || requiredEventTypes.Count <= 0)
                return events;

            return events.Where(x => requiredEventTypes.Any(required => required.IsInstanceOfType(x)));
        }
    }
}
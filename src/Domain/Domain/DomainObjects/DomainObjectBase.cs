using System;
using System.Collections.Generic;
using System.Linq;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Policies;
using BE.CQRS.Domain.States;
using BE.FluentGuard;

namespace BE.CQRS.Domain.DomainObjects
{
    public abstract class DomainObjectBase : IDomainObject
    {
        private const bool includeUncommittedStreamsDefault = false;
        private readonly List<IEvent> committedEvents = new List<IEvent>();
        private readonly List<IEvent> unCommittedEvents = new List<IEvent>();
        private readonly IEventMapper mapper;

        public string Id { get; }

        public bool HasUncommittedEvents => unCommittedEvents.Any();

        public List<IEvent> UnCommittedEvents => unCommittedEvents;

        public virtual bool CheckVersionOnSave { get; } = true;

        public virtual string Namespace { get; } = null;

        public long Version => committedEvents.Count + unCommittedEvents.Count;

        public long OriginVersion => committedEvents.Count;

        public long CommitVersion { get; private set; }

        protected DomainObjectBase(string id, IEventMapper mapper = null)
        {
            Precondition.For(id, nameof(id)).NotNullOrWhiteSpace();
            Id = id;
            this.mapper = mapper;
        }

        protected T RaiseEvent<T, U>(U mappingSource) where T : IEvent, new()
        {
            Precondition.For(mappingSource, nameof(mappingSource)).NotNull();
            Precondition.For(mapper, nameof(mapper)).NotNull();

            T @event = mapper.MapToEvent<U, T>(mappingSource);
            @event = RaiseEventInternal(@event, null);
            
            return @event;
        }

        protected T RaiseEvent<T, U>(U mappingSource, Action<T> modification) where T : IEvent, new()
        {
            Precondition.For(mappingSource, nameof(mappingSource)).NotNull();
            Precondition.For(modification, nameof(modification)).NotNull();
            Precondition.For(mapper, nameof(mapper)).NotNull();

            T @event = mapper.MapToEvent<U, T>(mappingSource);
            @event = RaiseEventInternal(@event, modification);

            return @event;
        }

        protected T RaiseEvent<T>(Action<T> modification) where T : IEvent, new()
        {
            Precondition.For(modification, nameof(modification)).NotNull();

            var @event = new T();
            @event = RaiseEventInternal(@event, modification);

            return @event;
        }

        protected T RaiseEvent<T>() where T : IEvent, new()
        {
            var @event = new T();
            @event = RaiseEventInternal(@event, null);

            return @event;
        }

        protected T RaiseEventInternal<T>(T @event, Action<T> modification) where T : IEvent, new()
        {
            SetEventDefaults(@event);

            modification?.Invoke(@event);

            @event.AssertValidation();
            unCommittedEvents.Add(@event);

            return @event;
        }

        private void SetEventDefaults<T>(T instance) where T : IEvent, new()
        {
            instance.Headers.Set(EventHeaderKeys.AggregateId, Id);
            instance.Headers.Set(EventHeaderKeys.Created, DateTimeOffset.Now);
        }

        public IReadOnlyCollection<IEvent> GetUncommittedEvents()
        {
            return unCommittedEvents;
        }

        public void CommitChanges(long commitVersion)
        {
            committedEvents.AddRange(unCommittedEvents);
            unCommittedEvents.Clear();
            CommitVersion = commitVersion;
        }

        public void RevertChanges()
        {
            unCommittedEvents.Clear();
        }

        public void ApplyEvents(ICollection<IEvent> eventsToCommit)
        {
            Precondition.For(eventsToCommit, nameof(eventsToCommit)).NotNull().True(i => i.Count > 0);

            ApplyCommitId(eventsToCommit);

            committedEvents.AddRange(eventsToCommit);
        }

        private void ApplyCommitId(IEnumerable<IEvent> eventsToCommit)
        {
            IEvent[] itemsWithCommitId = eventsToCommit.Where(i => i.Headers.HasKey(EventHeaderKeys.CommitId)).ToArray();

            if (itemsWithCommitId.Any())
            {
                CommitVersion = itemsWithCommitId.Max(x => x.Headers.GetInteger(EventHeaderKeys.CommitId));
            }
        }

        // TODO Extract Policy and StateExecutor
        public bool Policy<T>() where T : PolicyBase, new()
        {
            var state = new T();

            ExecuteState(includeUncommittedStreamsDefault, state);

            return state.IsValid();
        }

        public bool Policy<T>(ICommand command) where T : PolicyBase
        {
            return Policy(typeof(T), command);
        }

        private readonly Type commandPolicyType = typeof(CommandPolicyBase<>);

        public bool Policy(Type policy, ICommand command)
        {
            PolicyBase state;
            if (commandPolicyType.IsAssignableFrom(policy))
            {
                state = Activator.CreateInstance(policy, command) as PolicyBase;
            }
            else
            {
                state = Activator.CreateInstance(policy) as PolicyBase;
            }

            if (state == null)
            {
                throw new InvalidOperationException();
            }

            ExecuteState(includeUncommittedStreamsDefault, state);

            return state.IsValid();
        }

        public T State<T>() where T : StateBase, new()
        {
            return StateInternal<T>(includeUncommittedStreamsDefault);
        }

        public T State<T>(bool excludeUncommitted) where T : IState, new()
        {
            return StateInternal<T>(excludeUncommitted);
        }

        private T StateInternal<T>(bool excludeUncommitted) where T : IState, new()
        {
            var state = new T(); // TODO Include Di

            ExecuteState(excludeUncommitted, state);

            return state;
        }

        private void ExecuteState(bool excludeUncommitted, IState state)
        {
            var events = new List<IEvent>(committedEvents);

            if (!excludeUncommitted)
            {
                events.AddRange(unCommittedEvents);
            }

            state.Execute(events);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Configuration;
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
        private readonly IEventMapper mapper;

        private DomainObjectStateRuntime stateRuntime;

        public string Id { get; }

        public bool HasUncommittedEvents => UnCommittedEvents.Any();

        private List<IEvent> UnCommittedEvents { get; } = new List<IEvent>();

        public virtual bool CheckVersionOnSave { get; } = true;

        public virtual string Namespace { get; } = null;

        public long Version => committedEvents.Count + UnCommittedEvents.Count;

        public long OriginVersion => committedEvents.Count;

        public long CommitVersion { get; private set; }

        protected ISet<Type> LoadedEventTypes { get; private set; }

        protected DomainObjectBase(string id, IEventMapper mapper = null)
        {
            Precondition.For(id, nameof(id)).NotNullOrWhiteSpace();
            Id = id;
            this.mapper = mapper;
        }

        public void ApplyConfig(EventSourceConfiguration configuration)
        {
            stateRuntime = new DomainObjectStateRuntime(this, configuration);
        }

        public bool Policy<T>() where T : PolicyBase, new()
        {
            return stateRuntime.Policy<T>(includeUncommittedStreamsDefault);
        }

        public bool Policy<T>(ICommand command) where T : PolicyBase
        {
            return stateRuntime.Policy<T>(command, includeUncommittedStreamsDefault);
        }

        public bool Policy(Type policy, ICommand command)
        {
            return stateRuntime.Policy(policy, command, includeUncommittedStreamsDefault);
        }

        public T State<T>() where T : StateBase, new()
        {
            return stateRuntime.State<T>(includeUncommittedStreamsDefault);
        }

        public T State<T>(bool includeUnComitted) where T : StateBase, new()
        {
            return stateRuntime.State<T>(includeUnComitted);
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
            UnCommittedEvents.Add(@event);

            return @event;
        }

        private void SetEventDefaults<T>(T instance) where T : IEvent, new()
        {
            instance.Headers.Set(EventHeaderKeys.AggregateId, Id);
            instance.Headers.Set(EventHeaderKeys.Created, DateTimeOffset.Now);
        }

        public IReadOnlyCollection<IEvent> GetUncommittedEvents()
        {
            return UnCommittedEvents;
        }

        public IReadOnlyCollection<IEvent> GetCommittedEvents()
        {
            return committedEvents;
        }

        public void CommitChanges(long commitVersion)
        {
            committedEvents.AddRange(UnCommittedEvents);
            UnCommittedEvents.Clear();
            CommitVersion = commitVersion;
        }

        public void RevertChanges()
        {
            UnCommittedEvents.Clear();
        }

        public void ApplyEvents(ICollection<IEvent> eventsToCommit, ISet<Type> allowedEvents)
        {
            Precondition.For(eventsToCommit, nameof(eventsToCommit)).NotNull().True(i => i.Count > 0);

            LoadedEventTypes = allowedEvents;
            ApplyCommitId(eventsToCommit);

            if (allowedEvents != null && allowedEvents.Any())
                eventsToCommit = eventsToCommit.Where(x =>
                {
                    string eventType = x.Headers.GetString(EventHeaderKeys.AssemblyEventType);

                    return allowedEvents.Any(type => type.AssemblyQualifiedName.Equals(eventType));
                }).ToList();

            committedEvents.AddRange(eventsToCommit);
        }

        private void ApplyCommitId(IEnumerable<IEvent> eventsToCommit)
        {
            IEvent[] itemsWithCommitId = eventsToCommit.Where(i => i.Headers.HasKey(EventHeaderKeys.CommitId)).ToArray();

            if (itemsWithCommitId.Any())
                CommitVersion = itemsWithCommitId.Max(x => x.Headers.GetInteger(EventHeaderKeys.CommitId));
        }
    }
}
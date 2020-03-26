using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Policies;
using BE.CQRS.Domain.States;
using BE.FluentGuard;
using Microsoft.Extensions.DependencyInjection;

namespace BE.CQRS.Domain.DomainObjects
{
    public abstract class DomainObjectBase : IDomainObject
    {
        private const bool includeUncommittedStreamsDefault = false;
        private readonly List<IEvent> committedEvents = new List<IEvent>();
        private readonly IEventMapper mapper;

        private DomainObjectStateRuntime stateRuntime;
        private IDomainObjectRepository domainObjectRepository;

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

        public void ApplyConfig(EventSourceConfiguration configuration,EventsourceDIContext diContext,
            IStateEventMapping eventMapping,IDomainObjectRepository repo)
        {
            stateRuntime = new DomainObjectStateRuntime(this, diContext, eventMapping, configuration);

            domainObjectRepository = repo;
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

        public async Task ApplyEvents(IAsyncEnumerable<IEvent> eventsToCommit, ISet<Type> allowedEvents = null)
        {
            Precondition.For(eventsToCommit, nameof(eventsToCommit)).NotNull();
            LoadedEventTypes = allowedEvents;


            await foreach (IEvent @event in eventsToCommit)
            {
                ApplyEvent(@event, allowedEvents);
            }
        }

        public void ApplyEvent(IEvent @event, ISet<Type> allowedEvents = null)
        {
            if (@event.Headers.HasKey(EventHeaderKeys.CommitId))
            {
                var version = @event.Headers.GetLong(EventHeaderKeys.CommitId);
                CommitVersion = Math.Max(CommitVersion, version);
            }

            string eventType = @event.Headers.GetString(EventHeaderKeys.AssemblyEventType);
            if (allowedEvents == null ||
                (allowedEvents.Count > 0 && allowedEvents.Any(type =>
                    type != null && !string.IsNullOrWhiteSpace(type.AssemblyQualifiedName) &&
                    type.AssemblyQualifiedName.Equals(eventType))))
            {
                @committedEvents.Add(@event);
            }
        }

        public Task<TState> StateFor<TState>(string domainObjectId)
            where TState : StateBase, new()
        {
            return StateFor<TState>(domainObjectId, GetType());
        }

        public Task<TState> StateFor<TDomainObject, TState>(string domainObjectId)
            where TDomainObject : class, IDomainObject
            where TState : StateBase, new()
        {
            return StateFor<TState>(domainObjectId, typeof(TDomainObject));
        }

        public async Task<TState> StateFor<TState>(string domainObjectId, Type domainObjectType)
            where TState : StateBase, new()
        {
            IDomainObject domainObject = await domainObjectRepository.Get(domainObjectId, domainObjectType);

            return domainObject.State<TState>();
        }
    }
}
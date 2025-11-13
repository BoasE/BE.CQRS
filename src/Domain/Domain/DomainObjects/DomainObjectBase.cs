using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private const bool IncludeUncommittedStreamsDefault = false;
        private readonly List<IEvent> committedEvents = new ();
        private readonly IEventMapper mapper;

        private DomainObjectStateRuntime stateRuntime;
        private IDomainObjectRepository domainObjectRepository;
        protected TimeProvider Time { get; } = TimeProvider.System;
        public string Id { get; }

        public bool HasUncommittedEvents => UnCommittedEvents.Count > 0;

        private List<IEvent> UnCommittedEvents { get; } = new ();

        public virtual bool CheckVersionOnSave { get; } = false;

        public virtual string Namespace { get; } = null;

        public long Version => committedEvents.Count + UnCommittedEvents.Count;

        public long OriginVersion => committedEvents.Count;

        public long CommitVersion { get; private set; }

        private HashSet<string> allowedEventTypeNames;

        protected DomainObjectBase(string id, TimeProvider time = null, IEventMapper mapper = null)
        {
            Precondition.For(id, nameof(id)).NotNullOrWhiteSpace();
            Id = id;
            this.mapper = mapper;

            if (time != null)
            {
                Time = time;
            }
        }

        public void ApplyConfig(EventSourceConfiguration configuration, EventsourceDIContext diContext,
            IStateEventMapping eventMapping, IDomainObjectRepository repo)
        {
            stateRuntime = new DomainObjectStateRuntime(this, diContext, eventMapping, configuration);

            domainObjectRepository = repo;
        }

        public bool Policy<T>() where T : PolicyBase, new()
        {
            return stateRuntime.Policy<T>(IncludeUncommittedStreamsDefault);
        }

        public bool Policy<T>(ICommand command) where T : PolicyBase
        {
            return stateRuntime.Policy<T>(command, IncludeUncommittedStreamsDefault);
        }

        public bool Policy(Type policy, ICommand command)
        {
            return stateRuntime.Policy(policy, command, IncludeUncommittedStreamsDefault);
        }

        public T State<T>() where T : StateBase, new()
        {
            return stateRuntime.State<T>(IncludeUncommittedStreamsDefault);
        }

        public T State<T>(bool includeUnComitted) where T : StateBase, new()
        {
            return stateRuntime.State<T>(includeUnComitted);
        }

        protected T RaiseEvent<T, TSource>(TSource mappingSource) where T : IEvent, new()
        {
            Precondition.For(mappingSource, nameof(mappingSource)).NotNull();
            Precondition.For(mapper, nameof(mapper)).NotNull();

            T @event = mapper.MapToEvent<TSource, T>(mappingSource);
            @event = RaiseEventInternal(@event, null);

            return @event;
        }

        protected T RaiseEvent<T, TSource>(TSource mappingSource, Action<T> modification) where T : IEvent, new()
        {
            Precondition.For(mappingSource, nameof(mappingSource)).NotNull();
            Precondition.For(modification, nameof(modification)).NotNull();
            Precondition.For(mapper, nameof(mapper)).NotNull();

            T @event = mapper.MapToEvent<TSource, T>(mappingSource);
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
            instance.Headers.Set(EventHeaderKeys.Created, Time.GetLocalNow());
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

        // ReSharper disable PossibleMultipleEnumeration
        public async Task ApplyEvents(IAsyncEnumerable<IEvent> eventsToCommit, ISet<Type> allowedEvents = null)
        {
            Precondition.For(eventsToCommit, nameof(eventsToCommit)).NotNull();

            allowedEventTypeNames = null;
            if (allowedEvents is ICollection<Type> allowedCollection && allowedCollection.Count > 0)
            {
                allowedEventTypeNames = new HashSet<string>(StringComparer.Ordinal);
                foreach (var t in allowedCollection)
                {
                    var aqn = t?.AssemblyQualifiedName;
                    if (!string.IsNullOrWhiteSpace(aqn))
                        allowedEventTypeNames.Add(aqn);
                }
            }

            await foreach (IEvent @event in eventsToCommit)
            {
                ApplyEvent(@event);
            }
        }

        public void ApplyEvent(IEvent @event)
        {
            var headers = @event.Headers;
            if (headers.HasKey(EventHeaderKeys.CommitId))
            {
                var version = headers.GetLong(EventHeaderKeys.CommitId);
                CommitVersion = Math.Max(CommitVersion, version);
            }

            if (allowedEventTypeNames == null || allowedEventTypeNames.Count == 0)
            {
                committedEvents.Add(@event);
                return;
            }

            string eventType = headers.GetString(EventHeaderKeys.AssemblyEventType);
            if (!string.IsNullOrEmpty(eventType) && allowedEventTypeNames.Contains(eventType))
            {
                committedEvents.Add(@event);
            }
        }
        // ReSharper restore PossibleMultipleEnumeration

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
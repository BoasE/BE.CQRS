using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.FluentGuard;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain
{
    public abstract class DomainObjectRepositoryBase : IDomainObjectRepository
    {
        private readonly EventSourceConfiguration configuration;
        private static readonly string TraceCategory = typeof(DomainObjectRepositoryBase).FullName;
        private readonly ILogger logger;

        protected DomainObjectRepositoryBase(EventSourceConfiguration configuration)
        {
            Precondition.For(configuration, nameof(configuration))
                .NotNull("Configuration for domainobject repository must not be null!");

            this.configuration = configuration;
            logger = configuration.LoggerFactory.CreateLogger(GetType());
        }

        public async Task<AppendResult> SaveAsync<T>(T domainObject) where T : class, IDomainObject
        {
            AppendResult result = await SaveAsync(domainObject, false);

            AssertSave(domainObject, result);

            return result;
        }

        private void AssertSave<T>(T domainObject, AppendResult result) where T : class, IDomainObject
        {
            if (result.HadWrongVersion)
            {
                Type type = domainObject.GetType();
                string id = domainObject.Id;

                logger.LogError("Version conflict when saving a {type} with id {id}. Version {CurrentVersion}", type,
                    id,
                    result.CurrentVersion);

                throw new VersionConflictException(domainObject.GetType().Name, domainObject.Id,
                    result.CurrentVersion);
            }
        }

        public async Task<AppendResult> SaveAsync<T>(T domainObject, bool preventVersionCheck)
            where T : class, IDomainObject
        {
            Precondition.For(domainObject, nameof(domainObject))
                .NotNull("The domainObject to be saved must not be null!");

            string type = typeof(T).FullName;

            if (!domainObject.HasUncommittedEvents)
            {
                string id = domainObject.Id;
                logger.LogDebug("\"{type}\"-{id} had no events to save", type, id);

                return AppendResult.NoUpdate;
            }

            int count = domainObject.GetUncommittedEvents().Count;
            logger.LogTrace("Saving \"{type}\" with {count} events...", type, count);
            Stopwatch watch = Stopwatch.StartNew();

            bool check = domainObject.CheckVersionOnSave && !preventVersionCheck;
            AppendResult result = await SaveUncomittedEventsAsync(domainObject, check);

            domainObject.CommitChanges(result.CurrentVersion);
            watch.Stop();

            logger.LogTrace("Saved {count} events for \"{type}\" in {watch.ElapsedMilliseconds}ms", count, type, watch.ElapsedMilliseconds);
            return result;
        }

        public Task<long> GetVersion<T>(string id) where T : class, IDomainObject
        {
            string streamName = ResolveStreamName(id, typeof(T));

            return GetVersion(streamName);
        }

        public Task<long> GetVersion(Type domainObjectType, string id)
        {
            string streamName = ResolveStreamName(id, domainObjectType);

            return GetVersion(streamName);
        }

        protected abstract Task<long> GetVersion(string streamNaeme);

        public Task<bool> Exists<T>(T domainobject) where T : class, IDomainObject
        {
            return Exists<T>(domainobject.Id);
        }

        public Task<bool> Exists<T>(string id) where T : class, IDomainObject
        {
            string stream = ResolveStreamName(id, typeof(T));

            return ExistsStream(stream);
        }

        public IObservable<T> Get<T>(string id) where T : class, IDomainObject
        {
            return Get<T>(id, CancellationToken.None);
        }

        public IObservable<T> Get<T>(string id, ISet<Type> eventTypes, CancellationToken token) where T : class, IDomainObject
        {
            Type type = typeof(T);

            string streamName = ResolveStreamName(id, type);
            logger.LogTrace("Reading events for type \"{type}\"-{id} ...", type, id);

            return ReadEvents(streamName, eventTypes, token)
                .ToArray()
                .Select(events =>
                    {
                        var instance = configuration.Activator.Resolve<T>(id);

                        instance.ApplyEvents(events, eventTypes);
                        instance.ApplyConfig(configuration);
                        return instance;
                    }
                );
        }

        public IObservable<T> Get<T>(string id, CancellationToken token) where T : class, IDomainObject
        {
            Type type = typeof(T);

            string streamName = ResolveStreamName(id, type);
            logger.LogTrace("Reading events for type \"{type}\"-{id} ...", type, id);

            return ReadEvents(streamName, token)
                .ToArray()
                .Select(events =>
                    {
                        var instance = configuration.Activator.Resolve<T>(id);
                        instance.ApplyEvents(events, null);
                        instance.ApplyConfig(configuration);
                        return instance;
                    }
                );
        }

        public IObservable<IDomainObject> Get(string id, Type domainObjectType)
        {
            return Get(id, domainObjectType, CancellationToken.None);
        }

        public IObservable<IDomainObject> Get(string id, Type domainObjectType, CancellationToken token)
        {
            string streamName = ResolveStreamName(id, domainObjectType);

            return ReadEvents(streamName, token)
                .ToArray()
                .Select(events =>
                    {
                        IDomainObject instance = configuration.Activator.Resolve(domainObjectType, id);
                        instance.ApplyEvents(events, null);
                        instance.ApplyConfig(configuration);
                        return instance;
                    }
                );
        }

        protected abstract Task<bool> ExistsStream(string streamName);

        protected abstract string ResolveStreamName(string id, Type aggregateType);

        protected abstract Task<AppendResult> SaveUncomittedEventsAsync<T>(T domainObject, bool versionCheck)
            where T : class, IDomainObject;

        protected abstract IObservable<IEvent> ReadEvents(string streamName, CancellationToken token);

        protected abstract IObservable<IEvent> ReadEvents(string streamName, ISet<Type> eventTypes, CancellationToken token);

        public virtual IDomainObject New(Type domainObjectType, string id)
        {
            return configuration.Activator.Resolve(domainObjectType, id);
        }
    }
}
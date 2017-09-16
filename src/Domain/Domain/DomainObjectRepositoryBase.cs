using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain
{
    public abstract class DomainObjectRepositoryBase : IDomainObjectRepository
    {
        private readonly IDomainObjectActivator activator;
        private static readonly string TraceCategory = typeof(DomainObjectRepositoryBase).FullName;

        protected DomainObjectRepositoryBase(IDomainObjectActivator activator)
        {
            this.activator = activator;
        }

        public async Task<AppendResult> SaveAsync<T>(T domainObject) where T : class, IDomainObject
        {
            AppendResult result = await SaveAsync(domainObject, false);

            if (result.HadWrongVersion)
            {
                throw new VersionConflictException(domainObject.GetType().Name, domainObject.Id,
                    result.CurrentVersion);
            }
            return result;
        }

        public async Task<AppendResult> SaveAsync<T>(T domainObject, bool preventVersionCheck)
            where T : class, IDomainObject
        {
            string type = typeof(T).FullName;

            if (!domainObject.HasUncommittedEvents)
            {
                Trace.WriteLine("No events to save");
                return AppendResult.NoUpdate;
            }
            Trace.WriteLine($"Saving \"{type}\"...", TraceCategory);
            Stopwatch watch = Stopwatch.StartNew();

            bool check = domainObject.CheckVersionOnSave && !preventVersionCheck;
            AppendResult result = await SaveUncomittedEventsAsync(domainObject, check);

            domainObject.CommitChanges(result.CurrentVersion);
            watch.Stop();

            Trace.WriteLine($"Saved \"{type}\" in {watch.ElapsedMilliseconds}ms", TraceCategory);
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

        public IObservable<T> Get<T>(string id, CancellationToken token) where T : class, IDomainObject
        {
            Type type = typeof(T);

            string streamName = ResolveStreamName(id, type);
            Trace.WriteLine($"Reading \"{type}\" ...", TraceCategory);
            return ReadEvents(streamName, token)
                .ToArray()
                .Select(events =>
                    {
                        var instance = activator.Resolve<T>(id);
                        instance.ApplyEvents(events);

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
                        IDomainObject instance = activator.Resolve(domainObjectType, id);
                        instance.ApplyEvents(events);

                        return instance;
                    }
                );
        }

        protected abstract Task<bool> ExistsStream(string streamName);

        protected abstract string ResolveStreamName(string id, Type aggregateType);

        protected abstract Task<AppendResult> SaveUncomittedEventsAsync<T>(T domainObject, bool versionCheck)
            where T : class, IDomainObject;

        protected abstract IObservable<IEvent> ReadEvents(string streamName, CancellationToken token);

        public virtual IDomainObject New(Type domainObjectType, string id)
        {
            return activator.Resolve(domainObjectType, id);
        }
    }
}
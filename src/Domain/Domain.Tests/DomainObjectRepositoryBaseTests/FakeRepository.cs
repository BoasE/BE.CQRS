using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.Tests.DomainObjectRepositoryBaseTests
{
    public sealed class FakeRepository : DomainObjectRepositoryBase
    {
        public FakeRepository(EventSourceConfiguration configuration) : base(configuration)
        {
        }

        protected override Task<long> GetVersion(string streamNaeme)
        {
            return Task.FromResult(2L);
        }

        protected override Task<bool> ExistsStream(string streamName)
        {
            return Task.FromResult(true);
        }

        protected override string ResolveStreamName(string id, Type aggregateType)
        {
            return aggregateType.Name;
        }

        protected override Task<AppendResult> SaveUncomittedEventsAsync<T>(T domainObject, bool versionCheck)
        {
            return Task.FromResult(new AppendResult(false, 3));
        }

        protected override IObservable<IEvent> ReadEvents(string streamName, CancellationToken token)
        {
            var temp = new List<IEvent>();
            return temp.ToObservable();
        }

        protected override IObservable<IEvent> ReadEvents(string streamName, ISet<Type> eventTypes, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
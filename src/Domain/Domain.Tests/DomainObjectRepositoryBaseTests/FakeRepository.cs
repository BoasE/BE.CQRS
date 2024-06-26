﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.States;
using Xunit.Sdk;

namespace BE.CQRS.Domain.Tests.DomainObjectRepositoryBaseTests
{
    public sealed class FakeRepository : DomainObjectRepositoryBase
    {
        public FakeRepository(EventSourceConfiguration configuration, EventsourceDIContext diContext) : base(
            configuration, null, diContext, new StateEventMapping(), null)
        {
        }

        protected override Task<List<IEvent>> ByAppendResult(AppendResult result)
        {
            throw new NotImplementedException();
        }

        protected override Task RemoveStream(Type domainObjectType, string id)
        {
            throw new NotImplementedException();
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
            return Task.FromResult(new AppendResult("12", false, 3,""));
        }

        protected override IAsyncEnumerable<IEvent> ReadEvents(string streamName, CancellationToken token)
        {
            throw new InvalidOperationException();
            //throw new NotEmptyException("NoEvents"); //TODO How to return empty?
        }

        protected override IAsyncEnumerable<IEvent> ReadEvents(string streamName, long maxVersion,
            CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected override IAsyncEnumerable<IEvent> ReadEvents(string streamName, ISet<Type> eventTypes,
            CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public override IAsyncEnumerable<IEvent> EnumerateAll(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public override IAsyncEnumerable<IEvent> Enumerate(EnumerateDirection direction, int limit,
            CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
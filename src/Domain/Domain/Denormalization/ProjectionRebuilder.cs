using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Events.Handlers;

namespace BE.CQRS.Domain.Denormalization
{
    public sealed class ProjectionRebuilder : IProjectionRebuilder
    {
        private readonly IDomainObjectRepository repo;
        private readonly IEventHandler eventHandler;

        public ProjectionRebuilder(IDomainObjectRepository repo, IEventHandler eventHandler)
        {
            this.repo = repo;
            this.eventHandler = eventHandler;
        }

        public async Task Execute(CancellationToken token)
        {
            await foreach (var item in repo.EnumerateAll(token))
            {
                await Process(item);
            }
        }

        public async Task RebuildDomainobject<T>(string id) where T : class, IDomainObject
        {
            var domainObject = await repo.Get<T>(id);
            var events = domainObject.GetCommittedEvents().OrderBy(x => x.Headers.Created);

            foreach (var @event in events)
            {
                await eventHandler.HandleAsync(@event);
            }
        }

        private Task Process(IEvent @event)
        {
            return eventHandler.HandleAsync(@event);
        }
    }
}
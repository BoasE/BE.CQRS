using System.Threading.Tasks;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Denormalizations
{
    public sealed class MongoProjectedEventsRepository : IProjectedEventsRepository
    {
        private readonly IMongoDatabase db;

        public MongoProjectedEventsRepository(IMongoDatabase db)
        {
            this.db = db;
        }

        public Task<long> Increment<TDomainObject>(string domainObjectId, IEvent processesEvent)
            where TDomainObject : IDomainObject
        {
            throw new System.NotImplementedException();
        }

        public Task<long> Version<TDomainObject>(string domainObjectId) where TDomainObject : IDomainObject
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> WasProcessed<TDomainObject>(string domainObjectId, IEvent @event)
            where TDomainObject : IDomainObject
        {
            throw new System.NotImplementedException();
        }
    }
}
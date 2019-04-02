using System.Threading.Tasks;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Denormalizations
{
    public sealed class MongoReadVersionRepository : IReadVersionRepository
    {
        private readonly IMongoCollection<MongoDomainObjectVersion> collection;

        public MongoReadVersionRepository(IMongoDatabase db)
        {
            collection = db.GetCollection<MongoDomainObjectVersion>("DomainObjectReadVersions");
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
    }
}
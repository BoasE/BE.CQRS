using BE.CQRS.Data.MongoDb;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;
using MongoDB.Driver;

namespace RepositorySamples.MongoDb
{
    public static class CQRSBooter
    {
        public static IDomainObjectRepository AddCqrs(IMongoDatabase db)
        {
            var configuration = new EventSourceConfiguration();
            configuration.Activator = new ActivatorDomainObjectActivator();
            configuration.StateActivator = new ActivatorDomainObjectActivator();
            return new MongoDomainObjectRepository(configuration, db);
        }
    }
}
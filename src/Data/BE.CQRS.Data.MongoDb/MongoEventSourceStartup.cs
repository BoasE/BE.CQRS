using BE.CQRS.Domain.Configuration;
using BE.FluentGuard;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb
{
    public static class MongoEventSourceStartup
    {
        public static EventSourceConfiguration SetMongoDbEventSource(this EventSourceConfiguration config, IMongoDatabase db)
        {
            Precondition.For(() => db).NotNull();
            Precondition.For(() => config).NotNull();
            Precondition.For(() => config.Activator).NotNull();

            var repo = new MongoDomainObjectRepository(config.Activator, db);

            config.DomainObjectRepository = repo;
            return config;
        }
    }
}
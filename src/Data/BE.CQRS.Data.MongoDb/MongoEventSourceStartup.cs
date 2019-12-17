using BE.CQRS.Data.MongoDb.Streams;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Logging;
using BE.FluentGuard;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb
{
    public static class MongoEventSourceStartup
    {
        public static EventSourceConfiguration SetMongoDomainObjectRepository(this EventSourceConfiguration config,
            IMongoDatabase db)
        {
            Precondition.For(() => db).NotNull();
            Precondition.For(() => config).NotNull();
            Precondition.For(() => config.Activator).NotNull();

            var repo = new MongoDomainObjectRepository(config, db);

            config.DomainObjectRepository = repo;

            return config;
        }

        public static DenormalizerConfiguration SetMongoEventPositionGateway(this DenormalizerConfiguration config,
            IMongoDatabase db)
        {
            Precondition.For(() => config).NotNull();
            Precondition.For(() => db).NotNull();

            config.StreamPositionGateway = new MongoStreamPositionGateway(db, null);
            return config;
        }

        public static DenormalizerConfiguration SetMongoDbEventSubscriber(this DenormalizerConfiguration config,
            IMongoDatabase db, IEventHash hash, ILoggerFactory loggerFactory = null)
        {
            Precondition.For(() => config).NotNull();
            Precondition.For(() => db).NotNull();

            if (loggerFactory == null)
            {
                loggerFactory = new NoopLoggerFactory();
            }

            config.Subscriber = new MongoEventSubscriber(db, loggerFactory, hash);
            return config;
        }
    }
}
using System;
using BE.CQRS.Data.MongoDb.Streams;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Logging;
using BE.CQRS.Domain.Serialization;
using BE.FluentGuard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb
{
    public static class MongoEventSourceStartup
    {
        public static IServiceCollection AddMongoDomainObjectRepository(this IServiceCollection services,
            IMongoDatabase db)
        {
            Precondition.For(() => db).NotNull();
            
            services.AddSingleton<IDomainObjectRepository>(x => new MongoDomainObjectRepository(x.GetRequiredService<EventSourceConfiguration>(), db,
                x.GetRequiredService<IServiceProvider>()));
            
            return services;
        }

        public static IServiceCollection AddDefaultMongoDomainObjectRepository(this IServiceCollection services)
        {
            services.AddSingleton<IDomainObjectRepository>(x => new MongoDomainObjectRepository(x.GetRequiredService<EventSourceConfiguration>(), x.GetRequiredService<IMongoDatabase>(),
                x.GetRequiredService<IServiceProvider>()));
            
            return services;
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
            IMongoDatabase db, IEventHash hash,IEventSerializer eventSerializer, ILoggerFactory loggerFactory = null)
        {
            Precondition.For(() => config).NotNull();
            Precondition.For(() => db).NotNull();

            if (loggerFactory == null)
            {
                loggerFactory = new NoopLoggerFactory();
            }

            config.Subscriber = new MongoEventSubscriber(db, loggerFactory, hash,eventSerializer);
            return config;
        }
    }
}
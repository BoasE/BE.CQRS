﻿using System;
using BE.CQRS.Data.MongoDb.Streams;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Logging;
using BE.CQRS.Domain.Serialization;
using BE.FluentGuard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

            services.TryAddSingleton<IDomainObjectRepository>(x => new MongoDomainObjectRepository(
                x.GetRequiredService<EventSourceConfiguration>(), db,
                x.GetRequiredService<IServiceProvider>()));

            return services;
        }

        public static IServiceCollection AddDefaultMongoDomainObjectRepository(this IServiceCollection services)
        {
            services.TryAddSingleton<IDomainObjectRepository>(x => new MongoDomainObjectRepository(
                x.GetRequiredService<EventSourceConfiguration>(), x.GetRequiredService<IMongoDatabase>(),
                x.GetRequiredService<IServiceProvider>()));

            return services;
        }

        public static IServiceCollection AddMongoEventPositionGateway(this IServiceCollection services,
            IMongoDatabase db)
        {
            Precondition.For(() => db).NotNull();

            services.AddSingleton<IStreamPositionGateway, MongoStreamPositionGateway>();

            return services;
        }

        public static IServiceCollection AddMongoDbEventSubscriber(this IServiceCollection serivces,
            IMongoDatabase db)
        {
            Precondition.For(() => db).NotNull();

            serivces.AddSingleton<IEventSubscriber>(x => new MongoEventSubscriber(db,
                x.GetRequiredService<ILoggerFactory>(),
                x.GetRequiredService<IEventHash>(), x.GetRequiredService<IEventSerializer>()));
            return serivces;
        }
    }
}
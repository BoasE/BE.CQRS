﻿using System;
using BE.CQRS.Data.MongoDb.Streams;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using BE.CQRS.Domain.States;
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
            IMongoDatabase database,bool useTransactions=false,bool deactivateReadTimeout=false)
        {
            return AddMongoDomainObjectRepository(services, new MongoEventsourceDataContext(database,deactivateReadTimeout,useTransactions));
        }

        public static IServiceCollection AddMongoDomainObjectRepository(this IServiceCollection services,
            Func<IMongoDatabase> dbFactory,bool useTransactions=false,bool deactivateReadTimeout=false)
        {
            return AddMongoDomainObjectRepository(services, new MongoEventsourceDataContext(dbFactory,deactivateReadTimeout,useTransactions));
        }

        public static IServiceCollection AddMongoDomainObjectRepository(this IServiceCollection services,
            MongoEventsourceDataContext dataContext)
        {
            Precondition.For(() => dataContext).NotNull();

            services.TryAddSingleton<IDomainObjectRepository>(x => new MongoDomainObjectRepository(
                x.GetRequiredService<EventSourceConfiguration>(), dataContext,
                x.GetRequiredService<EventsourceDIContext>(), x.GetRequiredService<IEventSerializer>(),
                x.GetRequiredService<IEventHash>(),
                x.GetRequiredService<IImmediateConventionDenormalizerPipeline>(), x.GetRequiredService<IStateEventMapping>(),
                x.GetRequiredService<ILoggerFactory>()));

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
            IMongoDatabase db, bool useTransactions = false, bool deactivateTimeoutOnRead = false)
        {
            Precondition.For(() => db).NotNull();

            serivces.AddSingleton<IEventSubscriber>(x => new MongoEventSubscriber(db,
                x.GetRequiredService<ILoggerFactory>(),
                x.GetRequiredService<IEventHash>(), x.GetRequiredService<IEventSerializer>(), useTransactions,
                deactivateTimeoutOnRead));
            return serivces;
        }
    }
}
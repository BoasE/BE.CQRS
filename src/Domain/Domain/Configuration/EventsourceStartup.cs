using System;
using System.Linq;
using System.Reflection;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using BE.FluentGuard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Configuration
{
    public static class EventsourceStartup
    {
        public static EventSourceConfiguration AddEventSource(this IServiceCollection collection,
            EventSourceConfiguration config)
        {
            Precondition.For(() => config).NotNull();
            collection.AddSingleton(config);
            collection.TryAddSingleton<IEventTypeResolver, EventTypeResolver>();
            collection.TryAddSingleton<IEventSerializer, JsonEventSerializer>();
            collection.TryAddSingleton<IEventHash>(x =>
            {
                if (string.IsNullOrWhiteSpace(config.EventSecret))
                    throw new InvalidOperationException(
                        $"EventSecret must be set. Call \"{nameof(SetEventSecret)}\" before!");
                return new ShaEventHash(config.EventSecret);
            });

            return config;
        }


        public static EventSourceConfiguration SetDomainObjectAssemblies(this EventSourceConfiguration config,
            params Assembly[] assembliesWithDomainObjects)
        {
            Precondition.For(() => config).NotNull();
            Precondition.For(() => assembliesWithDomainObjects).NotNull().True(x => x.Any());

            config.DomainObjectAssemblies = assembliesWithDomainObjects;

            return config;
        }

        public static IServiceCollection AddEventSourceDefaults(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddConventionBasedInMemoryCommandBus(
            this IServiceCollection services, EventSourceConfiguration config)
        {
            services.AddSingleton<ICommandBus>(x => InMemoryCommandBus.CreateConventionCommandBus(
                x.GetRequiredService<IDomainObjectRepository>(),
                x.GetRequiredService<ILoggerFactory>(),
                config.DomainObjectAssemblies));

            return services;
        }

        public static EventSourceConfiguration SetEventSecret(this EventSourceConfiguration config, string secret)
        {
            Precondition.For(secret, nameof(secret)).NotNullOrWhiteSpace();
            config.EventSecret = secret;
            return config;
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using BE.FluentGuard;
using Microsoft.Extensions.DependencyInjection;
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

            if (config.EventHash == null)
                throw new InvalidOperationException($"EventHash must be set. Call \"{nameof(HashEvents)}\" before!");

            collection.AddSingleton(config.EventSerializer);
            collection.AddSingleton(config.EventHash);

            if (config.EventMapper != null)
                collection.AddSingleton(config.EventMapper);

            return config;
        }

        public static EventSourceConfiguration SetEventSerializer(this EventSourceConfiguration config,
            IEventSerializer serializer)
        {
            config.EventSerializer = serializer;

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

        public static EventSourceConfiguration HashEvents(this EventSourceConfiguration config, string secret)
        {
            Precondition.For(secret, nameof(secret)).NotNullOrWhiteSpace();
            config.EventHash = new ShaEventHash(secret);
            return config;
        }
    }
}
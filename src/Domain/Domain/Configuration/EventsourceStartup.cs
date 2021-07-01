using System;
using System.Linq;
using System.Reflection;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using BE.CQRS.Domain.States;
using BE.FluentGuard;
using Microsoft.AspNetCore.Builder;
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
            collection.TryAddSingleton<IStateEventMapping, StateEventMapping>();
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

            config.AddDomainObjectAssembly(assembliesWithDomainObjects);

            return config;
        }

        public static IApplicationBuilder UseEventSource(this IApplicationBuilder app)
        {
            var bus = app.ApplicationServices.GetRequiredService<ICommandBus>();

            return app;
        }

        public static IServiceCollection AddConventionBasedInMemoryCommandBus(
            this IServiceCollection services, EventSourceConfiguration config)
        {
            services.AddSingleton<EventSourceConfiguration>(config);
            services.AddSingleton<ICommandBus>(x => InMemoryCommandBus.CreateConventionCommandBus(
                x.GetRequiredService<IDomainObjectRepository>(),
                x.GetRequiredService<ILoggerFactory>(),
                x.GetRequiredService<EventSourceConfiguration>()));

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
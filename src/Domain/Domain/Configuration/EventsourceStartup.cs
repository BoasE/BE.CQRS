using System.Linq;
using System.Reflection;
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.DomainObjects;
using BE.FluentGuard;
using Microsoft.Extensions.DependencyInjection;

namespace BE.CQRS.Domain.Configuration
{
    public static class EventsourceStartup
    {
        public static EventSourceConfiguration AddEventSource(this IServiceCollection collection, EventSourceConfiguration config)
        {
            Precondition.For(() => config).NotNull();
            Precondition.For(() => config.Activator).NotNull();
            Precondition.For(() => config.CommandBus).NotNull();
            Precondition.For(() => config.DomainObjectRepository).NotNull();

            collection.AddSingleton(config.Activator);
            collection.AddSingleton(config.CommandBus);
            collection.AddSingleton(config.DomainObjectRepository);

            if (config.EventMapper != null)
            {
                collection.AddSingleton(config.EventMapper);
            }

            return config;
        }

        public static EventSourceConfiguration SetDomainObjectAssemblies(this EventSourceConfiguration config, params Assembly[] assembliesWithDomainObjects)
        {
            Precondition.For(() => config).NotNull();
            Precondition.For(() => assembliesWithDomainObjects).NotNull().True(x=>x.Any());

            config.DomainObjectAssemblies = assembliesWithDomainObjects;
            return config;
        }
        public static EventSourceConfiguration SetInMemoryCommandBus(this EventSourceConfiguration config)
        {
            Precondition.For(() => config).NotNull();
            Precondition.For(() => config.DomainObjectRepository).NotNull();
            Precondition.For(() => config.DomainObjectAssemblies).NotNull();

            config.CommandBus = InMemoryCommandBus.CreateConventionCommandBus(config.DomainObjectRepository,
                config.DomainObjectAssemblies);

            return config;
        }

        public static EventSourceConfiguration SetDefaultActivator(this EventSourceConfiguration config)
        {
            Precondition.For(() => config).NotNull();

            config.Activator = new ActivatorDomainObjectActivator();

            return config;
        }
    }
}
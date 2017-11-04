using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.DomainObjects;
using BE.FluentGuard;
using Microsoft.Extensions.DependencyInjection;

namespace BE.CQRS.Domain.Configuration
{
    public static class EventsourceStartup
    {
        public static void AddEventSource(this IServiceCollection collection, EventSourceConfiguration config)
        {
            Precondition.For(() => config).NotNull();
            Precondition.For(() => config.Activator).NotNull();
            Precondition.For(() => config.CommandBus).NotNull();
            Precondition.For(() => config.DomainObjectRepository).NotNull();

            collection.AddSingleton(config.Activator);
            collection.AddSingleton(config.CommandBus);
            collection.AddSingleton(config.DomainObjectRepository);
        }

        public static void SetInMemoryCommandBus(this EventSourceConfiguration config)
        {
            config.CommandBus = InMemoryCommandBus.CreateConventionCommandBus(config.DomainObjectRepository,
                config.DomainObjectAssemblies);
        }

        public static void SetDefaultActivator(this EventSourceConfiguration config)
        {
            config.Activator = new ActivatorDomainObjectActivator();
        }
    }
}
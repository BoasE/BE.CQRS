using System;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;

namespace BE.CQRS.Di.AspCore
{
    public static class EventsourceDiStartup
    {
        public static EventSourceConfiguration SetServiceCollectionActivator(this EventSourceConfiguration config, IServiceProvider serivces)
        {
            var activator = new ServiceCollectionActivator();

            config.Activator = activator;

            return config;
        }

        public static void UseServiceCollectionActivator(this IServiceProvider provider)
        {
            if (!(provider.GetService(typeof(IDomainObjectActivator)) is ServiceCollectionActivator activator))
            {
                throw new InvalidOperationException("UseServiceCollectionActivator requires a registered ServiceCollectionActivator");
            }

            activator.UseProvider(provider);
        }
    }
}
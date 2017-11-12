using System;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BE.CQRS.Di.AspCore
{
    public static class EventsourceDiStartup
    {
        public static EventSourceConfiguration SetServiceProviderActivator(this EventSourceConfiguration config)
        {
            var activator = new ServiceCollectionActivator();
            config.Activator = activator;

            return config;
        }

        public static void UseServiceProviderActivator(this IApplicationBuilder app)
        {
            if (!(app.ApplicationServices.GetRequiredService<IDomainObjectActivator>() is ServiceCollectionActivator activator))
            {
                throw new InvalidOperationException("UseServiceCollectionActivator requires a registered IDomainObjectActivator Service with a type of \"ServiceCollectionActivator\"");
            }

            activator.UseProvider(app.ApplicationServices.GetRequiredService<IServiceProvider>());
        }
    }
}
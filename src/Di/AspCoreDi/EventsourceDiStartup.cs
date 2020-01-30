using System;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Serialization;
using BE.FluentGuard;
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
            config.StateActivator = activator;
            return config;
        }

            public static DenormalizerConfiguration SetServiceProviderDenormalizerActivator(this DenormalizerConfiguration config)
        {
            Precondition.For(() => config).NotNull();

            config.Activator = new ServiceCollectionActivator();

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
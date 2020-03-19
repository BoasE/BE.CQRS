using System;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Serialization;
using BE.CQRS.Domain.States;
using BE.FluentGuard;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BE.CQRS.Di.AspCore
{
    public static class EventsourceDiStartup
    {
        public static IServiceCollection AddServiceProviderDomainObjectAcitvator(this IServiceCollection services)
        {
            services.AddSingleton<IDomainObjectActivator>(x =>
                    new ServiceCollectionActivator(x.GetRequiredService<IServiceProvider>()))
                .AddSingleton<IStateActivator>(x => (IStateActivator) x.GetRequiredService<IDomainObjectActivator>());

            return services;
        }

        public static DenormalizerConfiguration SetServiceProviderDenormalizerActivator(
            this DenormalizerConfiguration config)
        {
            Precondition.For(() => config).NotNull();

            config.Activator = new ServiceCollectionActivator();

            return config;
        }

        public static void UseServiceProviderActivator(this IApplicationBuilder app)
        {
            if (!(app.ApplicationServices.GetRequiredService<IDomainObjectActivator>() is ServiceCollectionActivator
                activator))
            {
                throw new InvalidOperationException(
                    "UseServiceCollectionActivator requires a registered IDomainObjectActivator Service with a type of \"ServiceCollectionActivator\"");
            }

            activator.UseProvider(app.ApplicationServices.GetRequiredService<IServiceProvider>());
        }
    }
}
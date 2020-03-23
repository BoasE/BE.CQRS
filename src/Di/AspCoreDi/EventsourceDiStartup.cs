using System;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Serialization;
using BE.CQRS.Domain.States;
using BE.FluentGuard;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

        public static IServiceCollection AddServiceProviderDenormalizerActivator(
            this IServiceCollection serivces)
        {
            Precondition.For(() => serivces).NotNull();

            serivces.TryAddSingleton<IDenormalizerActivator, ServiceCollectionActivator>();
            return serivces;
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
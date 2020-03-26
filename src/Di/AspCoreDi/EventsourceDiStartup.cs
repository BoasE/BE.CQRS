using System;
using BE.CQRS.Domain;
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
            services
                .TryAddSingleton<EventsourceDIContext>(x =>
                {
                    var activator = new ServiceCollectionActivator(x.GetRequiredService<IServiceProvider>());
                    return new EventsourceDIContext(activator, activator);
                });


            return services;
        }

        public static IServiceCollection AddServiceProviderDenormalizerActivator(
            this IServiceCollection serivces)
        {
            Precondition.For(() => serivces).NotNull();

            serivces.TryAddSingleton<DenormalizerDiContext>(x =>
                new DenormalizerDiContext(new ServiceCollectionActivator(x.GetRequiredService<IServiceProvider>())));

            return serivces;
        }
    }
}
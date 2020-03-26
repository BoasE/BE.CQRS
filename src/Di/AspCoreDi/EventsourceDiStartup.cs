using System;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Denormalization;
using BE.FluentGuard;
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
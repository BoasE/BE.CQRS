using System;
using BE.CQRS.Di.Unity;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Practices.Unity;

namespace BE.CQRS.Di.AspCore
{
    public static class EventsourceDiStartup
    {
        public static IServiceCollection AddUnityActivator(this IServiceCollection services, IUnityContainer container)
        {
            services
                .TryAddSingleton<EventsourceDIContext>(x =>
                {
                    var activator = new UnityDomainObjectActivator(container);
                    return new EventsourceDIContext(activator, activator);
                    
                });


            return services;
        }
    }
}
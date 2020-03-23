using System.Linq;
using System.Reflection;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.Events.Handlers;
using BE.CQRS.Domain.Logging;
using BE.FluentGuard;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BE.CQRS.Domain.Configuration
{
    public static class DenormalizerStartup
    {
        public static IServiceCollection AddDenormalizers(this IServiceCollection services,
            DenormalizerConfiguration config)
        {
            Precondition.For(() => services).NotNull();
            Precondition.For(() => config).NotNull();

            services.AddSingleton(config);

            return services;
        }

        /*public static EventDenormalizer UseConvetionBasedDenormalizer(this IApplicationBuilder app,
            ILoggerFactory logger) // TODO Extract Object Factory like the DomainObjectFacotry
        {
            Precondition.For(() => app).NotNull();

            var config = app.ApplicationServices.GetRequiredService<DenormalizerConfiguration>();

            var result = new EventDenormalizer(app.ApplicationServices.GetRequiredService<IEventSubscriber>(),
                new ConventionEventHandler(app.ApplicationServices.GetRequiredService<IDenormalizerActivator>(), logger,
                    config.DenormalizerAssemblies),
                config.StreamPositionGateway);

            return result;
        }*/

        public static DenormalizerConfiguration SetDenormalizerAssemblies(this DenormalizerConfiguration config,
            params Assembly[] assemblies)
        {
            Precondition.For(() => config).NotNull();
            Precondition.For(() => assemblies).NotNull().True(x => x.Any());

            config.DenormalizerAssemblies = assemblies;

            return config;
        }
    }
}
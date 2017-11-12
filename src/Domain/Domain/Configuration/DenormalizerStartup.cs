using System;
using System.Linq;
using System.Reflection;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.Events.Handlers;
using BE.FluentGuard;
using Microsoft.Extensions.DependencyInjection;

namespace BE.CQRS.Domain.Configuration
{
    public static class DenormalizerStartup
    {
        public static IServiceCollection AddDenormalizers(this IServiceCollection services, DenormalizerConfiguration config)
        {
            Precondition.For(() => services).NotNull();
            Precondition.For(() => config).NotNull();
            Precondition.For(() => config.Subscriber).NotNull();

            services.AddSingleton(config.Subscriber);

            return services;
        }

        public static DenormalizerConfiguration SetDenormalizerAssemblies(this DenormalizerConfiguration config, params Assembly[] assemblies)
        {
            Precondition.For(() => config).NotNull();
            Precondition.For(() => assemblies).NotNull().True(x => x.Any());

            config.DenormalizerAssemblies = assemblies;

            return config;
        }

        public static DenormalizerConfiguration SetDenormalizerFactory(this DenormalizerConfiguration config, Func<Type, object> factory)
        {
            Precondition.For(() => config).NotNull();
            Precondition.For(() => factory).NotNull();

            config.Factory = factory;

            return config;
        }

        public static DenormalizerConfiguration SetConvetionBasedDenormalizer(this DenormalizerConfiguration config)
        {
            Precondition.For(() => config).NotNull();
            Precondition.For(() => config.Subscriber).NotNull();
            Precondition.For(() => config.Factory).NotNull();
            Precondition.For(() => config.DenormalizerAssemblies).NotNull();
            Precondition.For(() => config.StreamPositionGateway).NotNull();

            config.EventDenormalizer = new EventDenormalizer(config.Subscriber, new ConventionEventHandler(config.Factory, config.DenormalizerAssemblies), config.StreamPositionGateway);
            return config;
        }
    }
}
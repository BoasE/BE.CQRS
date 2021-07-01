using BE.CQRS.Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BE.CQRS.Domain.Denormalization
{
    public static class DenormalizationBootstrap
    {
        public static IServiceCollection AddDenormalization(this IServiceCollection services,
            DenormalizerConfiguration config)
        {
            services.AddSingleton(config);
            services.TryAddSingleton<IImmediateConventionDenormalizerPipeline, NoopImmediateConventionDenormalizerPipeline>();
            return services;
        }

        public static IServiceCollection AddImmediateDenormalization(this IServiceCollection services)
        {
            services.AddSingleton<IImmediateConventionDenormalizerPipeline, ImmediateConventionDenormalizerPipeline>();
            return services;
        }

        public static IServiceCollection AddProjectionBuilder(this IServiceCollection services)
        {
            services.AddSingleton<IProjectionRebuilder, ProjectionRebuilder>();

            return services;
        }
    }
}
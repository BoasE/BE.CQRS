using BE.CQRS.Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BE.CQRS.Domain.Denormalization
{
    public static class DenormalizationBootstrap
    {
        public static IServiceCollection AddImmediateDenormalization(this IServiceCollection services,
            DenormalizerConfiguration config)
        {
            services.AddSingleton(config);
            services.AddSingleton<IImmediateConventionDenormalizer, ImmediateConventionDenormalizer>();
            return services;
        }

        public static IServiceCollection AddProjectionBuilder(this IServiceCollection services)
        {
            services.AddSingleton<IProjectionRebuilder, ProjectionRebuilder>();

            return services;
        }
    }
}
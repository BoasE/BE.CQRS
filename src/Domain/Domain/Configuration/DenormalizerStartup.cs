using System.Linq;
using System.Reflection;
using BE.FluentGuard;
using Microsoft.Extensions.DependencyInjection;

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
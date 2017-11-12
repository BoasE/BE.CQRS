using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BE.CQRS.Domain.Configuration
{
    public static class DenormalizerStartup
    {
        public static IServiceCollection AddDenormalizers(this IServiceCollection services,DenormalizerConfiguration config)
        {
            return services;
        }
    }
}
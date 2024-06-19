using AspCore.BackgroundDenormalization;
using BE.CQRS.Domain.Denormalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspCore
{
    public static class AspCoreDenormalizerStartup
    {
        public static IServiceCollection AddAspBackgroundDenormalization(this IServiceCollection services)
        {
            services
                .TryAddSingleton<IBackgroundEventQueue,InMemoryBackgroundEventQueue>();
                services.AddHostedService<AspBackgroundDenormalizerService>();
                
            return services;
        }
    }
}
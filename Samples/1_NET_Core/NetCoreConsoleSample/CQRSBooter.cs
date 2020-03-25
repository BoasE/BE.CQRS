using BE.CQRS.Data.MongoDb;
using BE.CQRS.Di.AspCore;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.DomainObjects;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NetCoreConsoleSample.Domain;

namespace NetCoreConsoleSample
{
    public static class CQRSBooter
    {
        public static IServiceCollection AddEventSource(this IServiceCollection services)
        {
            var cfg = new EventSourceConfiguration()
                .SetEventSecret("232") //Your secret key which is used to sign the vents
                .SetDomainObjectAssemblies(typeof(Customer).Assembly); //Tell the place where your domainobjects are

            services
                .AddServiceProviderDomainObjectAcitvator()
                .AddMongoDomainObjectRepository(() =>
                    new MongoClient("mongodb://localhost:27017/?connectTimeoutMS=10000")
                        .GetDatabase("eventTests2"))
                .AddConventionBasedInMemoryCommandBus(cfg)
                .AddEventSource(cfg);

            return services;
        }

        public static IServiceCollection AddDenormalizer(this IServiceCollection services)
        {
            DenormalizerConfiguration config = new DenormalizerConfiguration()
                .SetDenormalizerAssemblies(typeof(Customer).Assembly);

            services
                .AddServiceProviderDenormalizerActivator()
                .AddImmediateDenormalization()
                .AddDenormalization(config)
                .AddProjectionBuilder();

            return services;
        }
    }
}
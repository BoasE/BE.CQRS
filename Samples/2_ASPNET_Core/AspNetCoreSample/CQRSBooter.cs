using System;
using AspNetCoreSample.Domain;
using BE.CQRS.Data.MongoDb;
using BE.CQRS.Di.AspCore;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Denormalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace AspNetCoreSample
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
using System;
using BE.CQRS.Di.AspCore;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Denormalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace WebApplication.Bootstrap
{
    public static class DenormalizerStartup
    {
        public static IServiceCollection AddDenormalizers(this IServiceCollection services, IConfiguration config)
        {
            Console.WriteLine("Attaching Denormalizers...");
            string readDburl = config["read:Host"];
            string readdb = config["read:db"];

            var client = new MongoClient(readDburl);
            IMongoDatabase readDb = client.GetDatabase(readdb);

            var ctx = new DenormalizerContext(client, readDb);
            services.AddSingleton<IDenormalizerContext>(ctx);

            AddCqrsDenormalizer(services);

            Console.WriteLine("Denormalizers attached!");
            return services;
        }

        private static void AddCqrsDenormalizer(IServiceCollection services)
        {
            DenormalizerConfiguration deconfig = new DenormalizerConfiguration()
                .SetDenormalizerAssemblies(typeof(SampleDenormalizer).Assembly);

            services
                .AddServiceProviderDenormalizerActivator()
                .AddImmediateDenormalization()
                .AddDenormalization(deconfig) //<-- hier
                .AddProjectionBuilder();
        }
    }
}
using System;
using AspNetCoreSample.Domain;
using BE.CQRS.Data.MongoDb;
using BE.CQRS.Di.AspCore;
using BE.CQRS.Domain.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AspNetCoreSample
{
    public static class CQRSBooter
    {
        public static IServiceCollection AddCqrs(this IServiceCollection serivces, IConfiguration config)
        {
            Console.WriteLine("Adding CQRS...");
            string url = config["CustomerDatabase:MongoDb:Host"];
            string db = config["CustomerDatabase:MongoDb:Name"];

            Console.WriteLine($"ES DB: {url} - {db}");
            IMongoDatabase mongodb = new MongoClient(url).GetDatabase(db);
            serivces.AddEventSource(
                new EventSourceConfiguration()
                    .SetDomainObjectAssemblies(typeof(Customer).Assembly)
                    .SetServiceProviderActivator()
                    .SetMongoDomainObjectRepository(mongodb)
                    .SetConventionBasedInMemoryCommandBus());

            Console.WriteLine($"CQRS added");
            return serivces;
        }

        public static IApplicationBuilder UseCqrs(this IApplicationBuilder app)
        {
            app.UseServiceProviderActivator();
            return app;
        }
    }
}
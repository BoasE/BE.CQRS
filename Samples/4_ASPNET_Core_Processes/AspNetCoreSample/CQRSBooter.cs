using System;
using System.Threading.Tasks;
using AspNetCoreSample.Denormalizer;
using AspNetCoreSample.Domain;
using BE.CQRS.Data.MongoDb;
using BE.CQRS.Di.AspCore;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Denormalization;
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

        public static IServiceCollection AddCustomerDenormalizer(this IServiceCollection collection, IConfiguration config)
        {
            string url = config["CustomerDatabase:MongoDb:Host"];
            string db = config["CustomerDatabase:MongoDb:Name"];
            IMongoDatabase eventDb = new MongoClient(url).GetDatabase(db);

            IMongoDatabase readDb = GetReadDatabaseMongoDatabase(config, url);

            var deconfig = new DenormalizerConfiguration()
                .SetDenormalizerAssemblies(typeof(CustomerDenormalizer).Assembly)
                .SetMongoEventPositionGateway(readDb)
                .SetMongoDbEventSubscriber(eventDb)
                .SetServiceProviderDenormalizerActivator();

            collection.AddDenormalizers(deconfig);

            var ctx = new DenormalizerContext(readDb);
            collection.AddSingleton<IDenormalizerContext>(ctx);
            return collection;
        }


        private static IMongoDatabase GetReadDatabaseMongoDatabase(IConfiguration config, string url)
        {
            string readDburl = config["CustomerDatabase:MongoDb:Host"];
            string Readdb = config["CustomerDatabase:MongoDb:Name"];

            IMongoDatabase readDb = new MongoClient(url).GetDatabase(Readdb);
            return readDb;
        }

        public static async Task<IApplicationBuilder> UseCustomerDenormalizerAsync(this IApplicationBuilder app)
        {
            var cfg = app.ApplicationServices.GetRequiredService<DenormalizerConfiguration>();
            app.UseServiceProviderActivator();
            var foo = cfg.Activator as ServiceCollectionActivator;

            foo.UseProvider(app.ApplicationServices);

            EventDenormalizer denormalizer = app.UseConvetionBasedDenormalizer();

            await denormalizer.StartAsync(TimeSpan.FromMilliseconds(250));

            return app;
        }
    }
}
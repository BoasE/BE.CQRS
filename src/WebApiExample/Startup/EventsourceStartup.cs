using System;
using BE.CQRS.Domain.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace WebApiExample.Startup
{
    public static class EventSourceSetup
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services, IConfiguration config,
            ILoggerFactory logger)
        {
            Console.WriteLine("Adding CQRS...");
            string eventSecret = "0mDJVERJ34e4qLC6JYvT!$_d#+54d";
            string url = config["events:host"];
            string db = config["events:db"];

            services.AddServiceProviderAcitvator();
            Console.WriteLine($"ES DB: {url} - {db}");
            IMongoDatabase mongodb = new MongoClient(url).GetDatabase(db);

            EventSourceConfiguration esconfig = new EventSourceConfiguration()
                .HashEvents(eventSecret)
                .SetDomainObjectAssemblies(typeof(SampleDomainObject).Assembly)
                .SetServiceProviderActivator()
                .SetMongoDomainObjectRepository(mongodb)
                .SetConventionBasedInMemoryCommandBus();

            esconfig.StateActivator = new ServiceCollectionActivator();
            esconfig.LoggerFactory = logger;
            serivces.AddEventSource(esconfig);
            serivces.AddSingleton(esconfig);
            Console.WriteLine("CQRS added");
            return serivces;
        }
    }
}
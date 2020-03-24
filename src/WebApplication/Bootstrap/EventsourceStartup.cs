using System;
using BE.CQRS.Data.MongoDb;
using BE.CQRS.Di.AspCore;
using BE.CQRS.Domain.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WebApplication.Domain;

namespace WebApplication.Bootstrap
{
    public static class EventSourceSetup
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services, IConfiguration config)
        {
       
            string eventSecret = "0mDJVERJ34e4qLC6JYvT!$_d#+54d";
            var esconfig = new EventSourceConfiguration()
                .SetEventSecret(eventSecret)
                .SetDomainObjectAssemblies(typeof(DomainObjectSample).Assembly);

            string url = config["events:host"];
            string db = config["events:db"];
            
            services
                .AddServiceProviderDomainObjectAcitvator()
                .AddMongoDomainObjectRepository(()=>new MongoClient(url).GetDatabase(db))
                .AddConventionBasedInMemoryCommandBus(esconfig)
                .AddEventSource(esconfig);
           
            Console.WriteLine("CQRS added");
            return services;
        }
    }
}
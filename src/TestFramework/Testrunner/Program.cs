using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BE.CQRS.Data.MongoDb;
using BE.CQRS.Data.MongoDb.Streams;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Reactive;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using BE.CQRS.Di.AspCore;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Logging;
using BE.CQRS.Domain.Serialization;
using BE.CQRS.Domain.States;
using Serilog;

namespace Testrunner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            var dto = new MyEvent() {Id = "2"};
            dto = new MyEvent() {Id = "3"};
            dto.Headers.Set(EventHeaderKeys.AggregateId, "a");
            dto.Headers.Set(EventHeaderKeys.Created, DateTime.Now);

            var repo = serviceProvider.GetRequiredService<IDomainObjectRepository>();

            var bo2 = await repo.Get<SampleBo>("53fda695-5186-4253-a495-8f989d03dbf3");
            bo2.Next();

            Console.WriteLine("next");
            Console.ReadLine();

            var bo = new SampleBo(Guid.NewGuid().ToString());

            bo.ApplyConfig(serviceProvider.GetRequiredService<EventSourceConfiguration>(),
                serviceProvider.GetRequiredService<EventsourceDIContext>(),
                serviceProvider.GetRequiredService<IStateEventMapping>(),
                repo);
            bo.Execute();

            bo.Next();

            repo.SaveAsync(bo).Wait();
            repo.SaveAsync(bo).Wait();

            bo.Execute();
            bo.Execute();
            repo.SaveAsync(bo).Wait();


            Console.ReadLine();
        }


        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging(configure => configure.AddConsole());


            ConfigureEventSource(services);
            ConfigureDenormalizer(services);
        }

        private static void ConfigureEventSource(IServiceCollection services)
        {
            var cfg = new EventSourceConfiguration()
                .SetEventSecret("232")
                .SetDomainObjectAssemblies(typeof(SampleBo).Assembly);

            services
                .AddServiceProviderDomainObjectAcitvator()
                .AddMongoDomainObjectRepository(() =>
                    new MongoClient("mongodb://localhost:27017/?connectTimeoutMS=10000")
                        .GetDatabase("eventTests2"))
                .AddConventionBasedInMemoryCommandBus(cfg)
                .AddEventSource(cfg);
        }

        private static void ConfigureDenormalizer(IServiceCollection services)
        {
            DenormalizerConfiguration deconfig = new DenormalizerConfiguration()
                .SetDenormalizerAssemblies(typeof(TestDenormalizer).Assembly);

            services
                .AddServiceProviderDenormalizerActivator()
                .AddImmediateDenormalization()
                .AddDenormalization(deconfig)
                .AddProjectionBuilder();
        }
    }
}
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
using System.Threading;
using Serilog;

namespace Testrunner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var db = serviceProvider.GetRequiredService<IMongoDatabase>();

            var cfg = new EventSourceConfiguration()
            {
                Activator = new ActivatorDomainObjectActivator(),
                LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>(),
                StateActivator =  new ActivatorDomainObjectActivator()
            };

            var repo = new MongoDomainObjectRepository(cfg, db);

            var bo2 = await repo.Get<SampleBo>("53fda695-5186-4253-a495-8f989d03dbf3");
            bo2.Next();
            
            Console.WriteLine("next");
            Console.ReadLine();

            var bo = new SampleBo(Guid.NewGuid().ToString());
            bo.ApplyConfig(cfg);
            bo.Execute();

            bo.Next();

            repo.SaveAsync(bo).Wait();
            repo.SaveAsync(bo).Wait();

            bo.Execute();
            bo.Execute();
            repo.SaveAsync(bo).Wait();

            //bo = new SampleBo("1");
            // bo.Execute();

            //   repo.SaveAsync(bo).Wait();

            //bo = new SampleBo("2");
            //bo.Execute();
            //repo.SaveAsync(bo).Wait();

            //bo = new SampleBo("3");
            //bo.Execute();
            //bo.Next();
            //repo.SaveAsync(bo).Wait();
            //bo.CommitChanges();

            //bo.Next();
            //repo.SaveAsync(bo).Wait();

            //Console.WriteLine(repo.Exists<SampleBo>("1").Result.ToString(), " - ",
            //    repo.GetVersion<SampleBo>("1").Result);
            //Console.WriteLine(repo.Exists<SampleBo>("2").Result.ToString(), " - ",
            //    repo.GetVersion<SampleBo>("2").Result);
            //Console.WriteLine(repo.Exists<SampleBo>("3").Result.ToString(), " - ",
            //    repo.GetVersion<SampleBo>("3").Result);

            //Console.WriteLine(repo.Exists<SampleBo>("4").Result.ToString());

            //test(repo).Wait();

            Console.ReadLine();
        }

        private static void StartDenormalizer(IServiceProvider provider, params Assembly[] normalizerASsemblies)
        {
            var db = provider.GetRequiredService<IMongoDatabase>();
            var logger = provider.GetRequiredService<ILoggerFactory>();
            var subs = new MongoEventSubscriber(db, logger);
            var pos = new MongoStreamPositionGateway(db, null);
            var normalizerFactory = new Func<Type, object>(Activator.CreateInstance);

            var eventHandler = new ConventionEventHandler(normalizerFactory, normalizerASsemblies);
            var result = new EventDenormalizer(subs, eventHandler, pos);

            result.StartAsync(TimeSpan.FromSeconds(1)).Wait();
        }

        private static async Task test(MongoDomainObjectRepository repo)
        {
            SampleBo existing = await repo.Get<SampleBo>("3");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Trace);

            IMongoDatabase db =
                new MongoClient(
                        "mongodb+srv://localhost:27015")
                    .GetDatabase("eventTests2");

            services.AddSingleton<IMongoDatabase>(db);
        }
    }
}
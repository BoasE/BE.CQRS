using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BE.CQRS.Data.MongoDb;
using BE.CQRS.Data.MongoDb.Streams;
using BE.CQRS.Domain.Configuration;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.DomainObjects;
using BE.CQRS.Domain.Events.Handlers;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Testrunner
{
    class Program
    {
        static void Main(string[] args)
        {
            IMongoDatabase db =
                new MongoClient("mongodb://localhost:27017/?readPreference=primary").GetDatabase("eventTests");
            var repo = new MongoDomainObjectRepository(
                new EventSourceConfiguration() {Activator = new ActivatorDomainObjectActivator()}, db);

            StartDenormalizer(db, typeof(Program).GetTypeInfo().Assembly);

            Console.WriteLine("next");
            Console.ReadLine();
            var bo = new SampleBo("1");
            bo.Execute();

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

        private static void StartDenormalizer(IMongoDatabase db, params Assembly[] normalizerASsemblies)
        {
            
            var subs = new MongoEventSubscriber(db);
            var pos = new MongoStreamPositionGateway(db, null);
            var normalizerFactory = new Func<Type, object>(Activator.CreateInstance);

            var eventHandler = new ConventionEventHandler(normalizerFactory, normalizerASsemblies);
            var result = new EventDenormalizer(subs, eventHandler, pos);

            result.StartAsync(TimeSpan.FromSeconds(1)).Wait();
        }

        private static async Task test(MongoDomainObjectRepository repo)
        {
            SampleBo existing = await repo.Get<SampleBo>("3").FirstAsync();
        }
    }
}
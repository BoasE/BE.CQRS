using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BE.CQRS.Data.MongoDb.Commits;
using BE.CQRS.Data.MongoDb.Repositories;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb
{
    public sealed class MongoEventSubscriber : IEventSubscriber
    {
        private readonly EventMapper mapper = new EventMapper(new JsonEventSerializer(new EventTypeResolver()));
        private readonly MongoCommitRepository repo;
        private bool running;
        private readonly TimeSpan waitTime = TimeSpan.FromMilliseconds(250);
        private readonly TimeSpan idleTime = TimeSpan.FromMilliseconds(500);

        public string StreamName { get; } = "All";

        public MongoEventSubscriber(IMongoDatabase db)
        {
            repo = new MongoCommitRepository(db);
        }

        public IObservable<OccuredEvent> Start(long? position)
        {
            long ordinal = position ?? 0;
            running = true;

            return Observable.Create<OccuredEvent>(async observer =>
            {
                while (running)
                {
                    var work = false;

                    await repo.EnumerateStartingAfter(ordinal, commit =>
                    {
                        ordinal = commit.Ordinal;
                        foreach (IEvent @event in mapper.ExtractEvents(commit))
                        {
                            work = true;
                            var dto = new OccuredEvent(@event.Headers.GetLong(EventHeaderKeys.CommitId), @event);
                            observer.OnNext(dto);
                        }
                    });
                    await Delay(work);
                }

                observer.OnCompleted();
            });
        }

        private async Task Delay(bool work)
        {
            if (work)
            {
                await Task.Delay(waitTime);

                Console.WriteLine("had work");
            }
            else
            {
                Console.WriteLine("no work");
                await Task.Delay(idleTime);
            }
        }

        public void Stop()
        {
            running = false;
        }
    }
}
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BE.CQRS.Data.MongoDb.Commits;
using BE.CQRS.Data.MongoDb.Repositories;
using BE.CQRS.Domain.Denormalization;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using BE.FluentGuard;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb
{
    public sealed class MongoEventSubscriber : IEventSubscriber //TODO should be transformed to MongoDb Observable Collections
    {
        private readonly EventMapper mapper = new EventMapper(new JsonEventSerializer(new EventTypeResolver()));
        private readonly MongoCommitRepository repo;
        private bool running;
        private readonly TimeSpan waitTime = TimeSpan.FromMilliseconds(250);
        private readonly TimeSpan idleTime = TimeSpan.FromMilliseconds(500);
        private readonly ILogger<MongoEventSubscriber> logger;
        public string StreamName { get; } = "All";

        public MongoEventSubscriber(IMongoDatabase db, ILoggerFactory loggerFactory)
        {
            Precondition.For(db, nameof(db)).NotNull();
            Precondition.For(loggerFactory, nameof(loggerFactory)).NotNull();

            logger = loggerFactory.CreateLogger<MongoEventSubscriber>();
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
                    int count = 0;
                    var work = false;

                    await repo.EnumerateStartingAfter(ordinal, commit =>
                    {
                        ordinal = commit.Ordinal;
                        foreach (IEvent @event in mapper.ExtractEvents(commit))
                        {
                            work = true;
                            var dto = new OccuredEvent(@event.Headers.GetLong(EventHeaderKeys.CommitId), @event);
                            observer.OnNext(dto);
                            count++;
                        }
                    });

                    LogProcessedEvents(count);

                    await Delay(work);
                }

                observer.OnCompleted();
            });
        }

        private void LogProcessedEvents(int count)
        {
            if (count > 0)
            {
                logger.LogTrace("Processed {count} events", count);
            }
        }

        private async Task Delay(bool work)
        {
            if (work)
            {
                await Task.Delay(waitTime);
            }
            else
            {
                await Task.Delay(idleTime);
            }
        }

        public void Stop()
        {
            running = false;
        }
    }
}
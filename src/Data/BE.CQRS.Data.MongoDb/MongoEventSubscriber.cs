using System;
using System.Collections.Generic;
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
    public sealed class
        MongoEventSubscriber : IEventSubscriber //TODO should be transformed to MongoDb Observable Collections
    {
        private readonly EventMapper mapper;
        private readonly MongoCommitRepository repo;
        private bool running;
        private readonly TimeSpan waitTime = TimeSpan.FromMilliseconds(250);
        private readonly TimeSpan idleTime = TimeSpan.FromMilliseconds(500);
        private readonly ILogger<MongoEventSubscriber> logger;

        public string StreamName { get; } = "All";

        public MongoEventSubscriber(IMongoDatabase db, ILoggerFactory loggerFactory, IEventHash hash,
            IEventSerializer eventSerializer,
            bool useTransactions, bool deactivateTimeoutOnRead)
        {
            Precondition.For(db, nameof(db)).NotNull();
            Precondition.For(loggerFactory, nameof(loggerFactory)).NotNull();

            mapper = new EventMapper(eventSerializer, hash);
            logger = loggerFactory.CreateLogger<MongoEventSubscriber>();
            repo = new MongoCommitRepository(db, hash, eventSerializer,
                loggerFactory.CreateLogger<MongoCommitRepository>(),
                useTransactions, deactivateTimeoutOnRead);
        }

        public async IAsyncEnumerable<OccuredEvent> Start(long? position) //TODO Check Callers!
        {
            long ordinal = position ?? 0;
            running = true;
            while (running)
            {
                int count = 0;
                var work = false;

                await foreach (var commit in repo.EnumerateStartingAfter(ordinal))
                {
                    ordinal = commit.Ordinal;
                    foreach (IEvent @event in mapper.ExtractEvents(commit))
                    {
                        work = true;
                        var dto = new OccuredEvent(@event.Headers.GetLong(EventHeaderKeys.CommitId), @event);
                        yield return dto;
                        count++;
                    }

                    LogProcessedEvents(count);

                    await Delay(work);
                }
            }
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
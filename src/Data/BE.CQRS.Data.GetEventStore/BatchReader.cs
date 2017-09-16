using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Data.GetEventStore.Transformation;
using BE.CQRS.Domain.Events;
using EventStore.ClientAPI;

namespace BE.CQRS.Data.GetEventStore
{
    public sealed class BatchReader
    {
        private readonly IEventStoreConnection connection;
        private readonly IEventTransformer eventTransformer;

        private readonly int batchSize;

        public BatchReader(IEventStoreConnection connection, IEventTransformer transformer, int batchSize)
        {
            this.connection = connection;
            this.batchSize = batchSize;
            eventTransformer = transformer;
        }

        public async Task ExecuteAsync(string streamName, IObserver<IEvent> observer,
            CancellationToken cancellationToken)
        {
            var goOn = true;
            var iteration = 0;
            while (goOn && !cancellationToken.IsCancellationRequested)
            {
                int pos = iteration * batchSize;
                iteration++;
                StreamEventsSlice result = await connection.ReadStreamEventsForwardAsync(streamName, pos, batchSize, false);

                goOn = !result.IsEndOfStream;

                foreach (ResolvedEvent item in result.Events)
                {
                    IEvent @event = eventTransformer.FromResolvedEvent(item);

                    if (@event != null)
                    {
                        observer.OnNext(@event);
                    }
                    else
                    {
                        //If a event could not be created from the data, a placeholder event should be created to maintain the number and order of the total events
                        observer.OnNext(new SubstitutionEvent(item.OriginalStreamId, item.OriginalEventNumber, item.Event.Metadata, item.Event.Data));
                        HandleEventCreationFailure(item);
                    }
                }
            }

            observer.OnCompleted();
        }

        private static void HandleEventCreationFailure(ResolvedEvent @event)
        {
            Trace.WriteLine(
                $"Event {@event.OriginalEventNumber} from \"{@event.OriginalStreamId}\" could not be deserialized",
                "BE.CQRS.GetEventStore");
        }
    }
}
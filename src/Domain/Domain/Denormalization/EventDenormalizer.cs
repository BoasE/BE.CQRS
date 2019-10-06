using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BE.CQRS.Domain.Events.Handlers;

namespace BE.CQRS.Domain.Denormalization
{
    public sealed class EventDenormalizer
    {
        public int BoundProjectors => handler.HandlerCount;

        private readonly IEventSubscriber subscriber;
        private readonly IEventHandler handler;
        private readonly IStreamPositionGateway gtw;

        public EventDenormalizer(IEventSubscriber subscriber, IEventHandler handler, IStreamPositionGateway gtw)
        {
            this.subscriber = subscriber;
            this.handler = handler;
            this.gtw = gtw;
        }

        public async Task StartAsync(TimeSpan updateInvervall)
        {
            long? pos = await gtw.GetAsync(subscriber.StreamName);

            IAsyncEnumerable<OccuredEvent> eventStream = subscriber.Start(pos);


            Subject<OccuredEvent> feed = new Subject<OccuredEvent>();
            feed
                .Do(x => handler.HandleAsync(x.Event).Wait())
                .Buffer(updateInvervall)
                .Subscribe(i => UpdatePosition(i).Wait());

            await foreach (var @event in eventStream)
            {
                feed.OnNext(@event);
            }

            feed.OnCompleted();
        }

        private async Task UpdatePosition(IList<OccuredEvent> i)
        {
            if (i.Any())
            {
                long result = i.Max(x => x.Pos);
                await gtw.SaveAsync(subscriber.StreamName, result);
            }
        }
    }
}
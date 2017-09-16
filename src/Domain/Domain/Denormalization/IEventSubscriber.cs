using System;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IEventSubscriber
    {
        IObservable<OccuredEvent> Start(long? position);

        void Stop();

        string StreamName { get; }
    }
}
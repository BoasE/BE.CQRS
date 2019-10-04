using System;
using System.Collections.Generic;

namespace BE.CQRS.Domain.Denormalization
{
    public interface IEventSubscriber
    {
        IAsyncEnumerable<OccuredEvent> Start(long? position);

        void Stop();

        string StreamName { get; }
    }
}
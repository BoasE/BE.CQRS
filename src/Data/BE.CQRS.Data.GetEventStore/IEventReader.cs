using System;
using System.Threading;
using System.Threading.Tasks;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Data.GetEventStore
{
    public interface IEventReader
    {
        Task<bool> ExistsStreamAsync(string streamName);

        IObservable<IEvent> ReadEvents(string streamName, CancellationToken token);

        IObservable<IEvent> ReadEvents(string streamName);

        Task<long> ReadVersion(string streamName);
    }
}
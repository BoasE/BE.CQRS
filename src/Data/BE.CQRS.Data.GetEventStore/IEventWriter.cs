using System.Collections.Generic;
using System.Threading.Tasks;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Data.GetEventStore
{
    public interface IEventWriter
    {
        Task<AppendResult> AppendAsync(string streamName, IEnumerable<IEvent> events, long expectedVersion);
    }
}
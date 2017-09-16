using BE.CQRS.Domain.Events;
using EventStore.ClientAPI;

namespace BE.CQRS.Data.GetEventStore.Transformation
{
    public interface IEventTransformer
    {
        EventData ToEventData(IEvent @event);

        IEvent FromResolvedEvent(ResolvedEvent @event);
    }
}
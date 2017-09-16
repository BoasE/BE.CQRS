using System.Collections.Generic;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.Serialization
{
    public interface IEventSerializer
    {
        IEvent DeserializeEvent(string headerData, string eventData);

        IEvent DeserializeEvent(Dictionary<string, string> headerData, string eventData);

        string SerializeHeader(EventHeader headers);

        string SerializeEvent(IEvent @event);
    }
}
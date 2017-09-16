using BE.CQRS.Data.GetEventStore.Tests.EventSerializerTests;
using BE.CQRS.Data.GetEventStore.Transformation;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using EventStore.ClientAPI;
using Xunit;

namespace BE.CQRS.Data.GetEventStore.Tests.EventTransformatorTests
{
    public class GivenEventTransformator
    {
        public EventTransformator GetSut()
        {
            return new EventTransformator(new JsonEventSerializer(new EventTypeResolver()));
        }

        [Fact]
        public void ItCreatesEventData()
        {
            EventTransformator sut = GetSut();

            var @event = new SimpleEvent();

            EventData data = sut.ToEventData(@event);
        }
    }
}
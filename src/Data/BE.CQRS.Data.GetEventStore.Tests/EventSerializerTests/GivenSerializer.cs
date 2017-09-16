using System;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using Xunit;

namespace BE.CQRS.Data.GetEventStore.Tests.EventSerializerTests
{
    public class GivenSerializer
    {
        private const string Prefix = nameof(GivenSerializer) + ".";

        public JsonEventSerializer GetSut()
        {
            return new JsonEventSerializer(new EventTypeResolver());
        }

        [Fact(DisplayName = Prefix + nameof(ItThrowsWhenNull))]
        public void ItThrowsWhenNull()
        {
            JsonEventSerializer sut = GetSut();

            SimpleEvent foo = null;
            Assert.Throws<ArgumentNullException>(() => sut.SerializeEvent(foo));
        }

        [Fact(DisplayName = Prefix + nameof(ItSerializes))]
        public void ItSerializes()
        {
            JsonEventSerializer sut = GetSut();

            var @event = new SimpleEvent
            {
                Value = "Foo"
            };

            @event.Headers.Set("abc", "foo");

            string result = sut.SerializeEvent(@event);
            string header = sut.SerializeHeader(@event.Headers);

            IEvent second = sut.DeserializeEvent(header, result);

            string temp = second.Headers.GetString("abc");

            Assert.Equal("foo", temp);
            Assert.Equal("Foo", @event.Value);
        }
    }
}
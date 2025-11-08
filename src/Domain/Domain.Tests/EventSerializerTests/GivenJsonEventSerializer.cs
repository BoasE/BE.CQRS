using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using Xunit;

namespace BE.CQRS.Domain.Tests.EventSerializerTests
{
    public class GivenJsonEventSerializer
    {
        protected JsonEventSerializer GetSut()
        {
            return new JsonEventSerializer(new EventTypeResolver());
        }

        [Fact]
        public void WhenSerializedAndDeserializedEventHasSomeBody()
        {
            var sut = GetSut();

            var source = new SampleEvent();
            var text = sut.SerializeEvent(source);
            var header = sut.SerializeHeader(source.Headers);

            var result = (SampleEvent)sut.DeserializeEvent(header, text);

            Assert.Equal(source.Text, result.Text);
            Assert.Equal(source.Value, result.Value);
        }

        [Fact]
        public void WhenSerializedAndDeserializedEventHasSomeHeader()
        {
            var sut = GetSut();

            var source = new SampleEvent();
            var text = sut.SerializeEvent(source);
            var header = sut.SerializeHeader(source.Headers);

            var result = (SampleEvent)sut.DeserializeEvent(header, text);

            Assert.Equal(source.Headers.Count, result.Headers.Count);
        }

        [Fact]
        public void WhenDeserializingInvalidJsonThrowsEventSerializationException()
        {
            var sut = GetSut();

            var source = new SampleEvent();
            source.Headers.SetAggregateId("test-aggregate-123");
            var header = sut.SerializeHeader(source.Headers);
            var invalidJson = "{invalid json data}";

            var exception = Assert.Throws<EventSerializationException>(() => 
                sut.DeserializeEvent(header, invalidJson));

            Assert.NotNull(exception.InnerException);
            Assert.Equal("test-aggregate-123", exception.AggregateId);
            Assert.Equal(invalidJson, exception.EventData);
        }

        [Fact]
        public void WhenDeserializationFailsExceptionContainsEventType()
        {
            var sut = GetSut();

            var source = new SampleEvent();
            source.Headers.SetAggregateId("test-aggregate-456");
            var header = sut.SerializeHeader(source.Headers);
            var invalidJson = "null";

            var exception = Assert.Throws<EventSerializationException>(() => 
                sut.DeserializeEvent(header, invalidJson));

            Assert.NotNull(exception.EventType);
            Assert.Contains("SampleEvent", exception.EventType);
        }

        [Fact]
        public void WhenDeserializationFailsExceptionContainsAggregateId()
        {
            var sut = GetSut();

            var aggregateId = "my-special-aggregate-789";
            var source = new SampleEvent();
            source.Headers.SetAggregateId(aggregateId);
            var header = sut.SerializeHeader(source.Headers);
            var invalidJson = "{broken}";

            var exception = Assert.Throws<EventSerializationException>(() => 
                sut.DeserializeEvent(header, invalidJson));

            Assert.Equal(aggregateId, exception.AggregateId);
        }
    }
}
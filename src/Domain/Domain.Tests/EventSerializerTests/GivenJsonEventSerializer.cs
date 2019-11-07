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
    }
}
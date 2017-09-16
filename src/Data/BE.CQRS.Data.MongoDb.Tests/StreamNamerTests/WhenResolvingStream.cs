using Xunit;

namespace BE.CQRS.Data.MongoDb.Tests.StreamNamerTests
{
    public class WhenResolvingStream : GivenStreamNamer
    {
        private readonly string streamName;

        public WhenResolvingStream()
        {
            streamName = sut.ResolveStreamName("123", typeof(SampleBo));
        }

        [Fact]
        public void ItContainsTheId()
        {
            Assert.True(streamName.Contains("123"));
        }

        [Fact]
        public void ItContainsTheType()
        {
            Assert.True(streamName.Contains(typeof(SampleBo).FullName));
        }

        [Fact]
        public void ItCanReConstructId()
        {
            string id = sut.IdByStreamName(streamName);

            Assert.Equal("123", id);
        }

        [Fact]
        public void ItCanReConstructTheTypeName()
        {
            string type = sut.TypeNameByStreamName(streamName);

            Assert.Equal(typeof(SampleBo).FullName, type);
        }
    }
}
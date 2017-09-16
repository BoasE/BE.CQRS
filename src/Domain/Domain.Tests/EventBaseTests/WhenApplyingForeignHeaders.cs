using Xunit;

namespace BE.CQRS.Domain.Tests.EventBaseTests
{
    public sealed class WhenApplyingForeignHeaders : GivenEvent
    {
        [Fact]
        public void ItHasSameId()
        {
            var @event = new SampleEvent(sampleEvent.Headers);

            Assert.Equal(sampleEvent.Headers.EventId, @event.Headers.EventId);
        }
    }
}
using BE.CQRS.Domain.Events;
using Xunit;

namespace BE.CQRS.Domain.Tests.EventBaseTests
{
    public class GivenEvent
    {
        protected readonly SampleEvent sampleEvent;

        public GivenEvent()
        {
            sampleEvent = GetSut();
        }

        protected SampleEvent GetSut()
        {
            return new SampleEvent();
        }

        [Fact]
        public void ItHasTheEventType()
        {
            string name = sampleEvent.GetType().Name;

            Assert.Equal(name, sampleEvent.Headers.GetString(EventHeaderKeys.EventType));
        }

        [Fact]
        public void ItHasAHeader()
        {
            Assert.NotNull(sampleEvent.Headers);
        }
    }
}
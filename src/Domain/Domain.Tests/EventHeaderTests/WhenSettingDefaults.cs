using System;
using BE.CQRS.Domain.Events;
using Xunit;

namespace BE.CQRS.Domain.Tests.EventHeaderTests
{
    public class WhenSettingDefaults : GivenEventHeader
    {
        private readonly EventHeader sut;

        public WhenSettingDefaults()

        {
            EventHeader sut = GetSut();
            sut.SetDetaults("123");
            this.sut = sut;
        }

        [Fact]
        public void ItHasTheId()
        {
            string id = sut.GetString(EventHeaderKeys.AggregateId);

            Assert.Equal("123", id);
        }

        [Fact]
        public void ItHasEventId()
        {
            Assert.True(sut.EventId != Guid.Empty);
        }
    }
}
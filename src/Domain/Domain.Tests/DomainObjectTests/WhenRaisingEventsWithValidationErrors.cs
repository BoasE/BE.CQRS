using System;
using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectTests
{
    public class WhenRaisingEventsWithValidationErrors : GivenDomainObject
    {
        private readonly TestDomainObject sut;

        public WhenRaisingEventsWithValidationErrors()
        {
            sut = GetSut("22");
        }

        [Fact]
        public void ItThrows()
        {
            Assert.Throws<InvalidOperationException>(() => sut.RaiseInvalidEvent());
        }

        [Fact]
        public void ItHasNoPendingEvents()
        {
            try
            {
                sut.RaiseInvalidEvent();
            }
            catch
            {
            }
            finally
            {
                Assert.False(sut.HasUncommittedEvents);
            }
        }
    }
}
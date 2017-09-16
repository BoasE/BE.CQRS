using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectTests
{
    public class WhenRevertingChanges : GivenDomainObject
    {
        [Fact]
        public void ItDoesntDoAnythingWithoutChanges()
        {
            TestDomainObject sut = GetSut("aa");

            sut.RevertChanges();

            Assert.Equal(0, sut.Version);
            Assert.Equal(0, sut.OriginVersion);
        }

        [Fact]
        public void WhenRevertingEvents()
        {
            TestDomainObject sut = GetSut("aa");
            sut.RaiseEvent();
            sut.RevertChanges();

            Assert.Equal(0, sut.Version);
            Assert.Equal(0, sut.OriginVersion);
        }
    }
}
using Xunit;

namespace BE.CQRS.Domain.Tests.DomainObjectTests
{
    public class WhenCommitingNoEvents : GivenDomainObject
    {
        private readonly TestDomainObject sut;

        public WhenCommitingNoEvents()
        {
            sut = GetSut("22");
            sut.CommitChanges(2);
        }

        [Fact]
        public void ItHasNotChangedVersion()
        {
            Assert.Equal(0, sut.Version);
        }

        [Fact]
        public void ItHasNotChangedOriginVersion()
        {
            Assert.Equal(0, sut.OriginVersion);
        }
    }
}
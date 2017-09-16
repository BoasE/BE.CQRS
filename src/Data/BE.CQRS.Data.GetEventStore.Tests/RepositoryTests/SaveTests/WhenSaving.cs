using System.Threading.Tasks;
using EventStore.ClientAPI;
using FakeItEasy;
using Xunit;

namespace BE.CQRS.Data.GetEventStore.Tests.RepositoryTests.SaveTests
{
    public sealed class WhenSaving : GivenEventStore
    {
        [Fact]
        public async Task ItCallsTheConnections()
        {
            var con = A.Fake<IEventStoreConnection>();

            EventStoreRepository sut = GetSut("UnitTest", con);

            var domainObj = new SampleDomainObject("1");
            await sut.SaveAsync(domainObj);
        }
    }
}
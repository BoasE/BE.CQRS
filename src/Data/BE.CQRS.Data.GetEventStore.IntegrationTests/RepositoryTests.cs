using System.Reactive.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Xunit;

namespace BE.CQRS.Data.GetEventStore.IntegrationTests
{
    public class RepositoryTests
    {
        public async Task<EventStoreRepository> GetSut()
        {
            IEventStoreConnection con = await LocalConfig.GetConnection();
            EventStoreContext context = EventStoreContext.CreateDefault("aa", con);

            return new EventStoreRepository(context);
        }

        [Fact]
        public async Task Create()
        {
            EventStoreRepository sut = await GetSut();
            var foo = new SampleDomainObject("a04s4ac4-16eb-4b6b-aeea-12b2e034a408");

            foo.Add();
            await sut.SaveAsync(foo);
        }

        [Fact]
        public async Task Update()
        {
            var id = "a04s4ac4-16eb-4b6b-aeea-12b2e034a408";
            EventStoreRepository sut = await GetSut();

            SampleDomainObject result = await sut.Get<SampleDomainObject>(id);
            result.Add();

            await sut.SaveAsync(result);
        }
    }
}
using System.Threading.Tasks;
using BE.CQRS.Data.GetEventStore.Transformation;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using Xunit;

namespace BE.CQRS.Data.GetEventStore.IntegrationTests
{
    public class StreamReaderTests
    {
        private async Task<EventStoreReader> GetSut()
        {
            var transformer = new EventTransformator(new JsonEventSerializer(new EventTypeResolver()));
            var reader = new EventStoreReader(await LocalConfig.GetConnection(), transformer);

            return reader;
        }

        [Fact]
        public async Task Test1()
        {
            EventStoreReader reader = await GetSut();

            bool result = await reader.ExistsStreamAsync("aa");
        }
    }
}
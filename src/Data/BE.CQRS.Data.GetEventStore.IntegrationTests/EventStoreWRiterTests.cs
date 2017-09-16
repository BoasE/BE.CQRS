using System.Collections.Generic;
using System.Threading.Tasks;
using BE.CQRS.Data.GetEventStore.Transformation;
using BE.CQRS.Domain;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Serialization;
using EventStore.ClientAPI;
using Xunit;

namespace BE.CQRS.Data.GetEventStore.IntegrationTests
{
    public class EventStoreWRiterTests
    {
        public async Task<EventStoreWriter> GetSut()
        {
            IEventStoreConnection con = await LocalConfig.GetConnection();
            var serializer = new JsonEventSerializer(new EventTypeResolver());
            var transformer = new EventTransformator(serializer);

            var sut = new EventStoreWriter(con, transformer);
            return sut;
        }

        [Fact]
        public async Task Append()
        {
            EventStoreWriter sut = await GetSut();

            var @event = new SimpleEvent();

            await sut.AppendAsync("foo-1", new List<IEvent>
            {
                @event
            }, DomainObjectVersion.Any);
        }
    }
}
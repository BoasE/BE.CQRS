using BE.CQRS.Data.GetEventStore.Transformation;
using EventStore.ClientAPI;

namespace BE.CQRS.Data.GetEventStore.Tests.BatchReaderTests
{
    public class GivenBatchReader
    {
        protected int batchSize { get; } = 5;

        protected BatchReader GivenSut(IEventStoreConnection connection, IEventTransformer transformer)
        {
            return new BatchReader(connection, transformer, batchSize);
        }
    }
}
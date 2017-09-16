using BE.CQRS.Data.MongoDb.Streams;

namespace BE.CQRS.Data.MongoDb.Tests.StreamNamerTests
{
    public class GivenStreamNamer
    {
        protected StreamNamer sut { get; } = new StreamNamer();
    }
}
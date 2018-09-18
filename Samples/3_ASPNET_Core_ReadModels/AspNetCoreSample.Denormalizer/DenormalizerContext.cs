using MongoDB.Driver;

namespace AspNetCoreSample.Denormalizer
{
    public interface IDenormalizerContext
    {
        IMongoDatabase Db { get; }
    }

    public sealed class DenormalizerContext : IDenormalizerContext
    {
        public IMongoDatabase Db { get; }

        public DenormalizerContext(IMongoDatabase db)
        {
            Db = db;
        }
    }
}
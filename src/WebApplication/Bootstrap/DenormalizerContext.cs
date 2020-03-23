using MongoDB.Driver;

namespace WebApplication.Bootstrap
{
    public interface IDenormalizerContext
    {
        IMongoDatabase Db { get; }

        IMongoClient Client { get; }
    }

    public sealed class DenormalizerContext : IDenormalizerContext
    {
        public DenormalizerContext(IMongoClient client, IMongoDatabase db)
        {
            Client = client;
            Db = db;
        }

        public IMongoDatabase Db { get; }

        public IMongoClient Client { get; }
    }
}
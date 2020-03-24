using System;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb
{
    public sealed class MongoEventsourceDataContext
    {
        private readonly Lazy<IMongoDatabase> factory;

        public IMongoDatabase Database => factory.Value;

        public MongoEventsourceDataContext(IMongoDatabase db)
        {
            factory = new Lazy<IMongoDatabase>(() => db);
        }

        public MongoEventsourceDataContext(Func<IMongoDatabase> factory)
        {
            this.factory = new Lazy<IMongoDatabase>(factory);
        }
    }
}
using System;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb
{
    public sealed class MongoEventsourceDataContext
    {
        private readonly Lazy<IMongoDatabase> factory;

        public IMongoDatabase Database => factory.Value;

        public bool DeactivateTimoutOnCommitScan { get; }
        public bool UseTransactions { get; }

        public MongoEventsourceDataContext(IMongoDatabase db, bool deactivateTimoutOnCommitScan = false,
            bool useTransactions = false)
        {
            DeactivateTimoutOnCommitScan = deactivateTimoutOnCommitScan;
            UseTransactions = useTransactions;

            factory = new Lazy<IMongoDatabase>(() => db);
        }

        public MongoEventsourceDataContext(Func<IMongoDatabase> factory, bool deactivateTimoutOnCommitScan = false,
            bool useTransactions = false)
        {
            DeactivateTimoutOnCommitScan = deactivateTimoutOnCommitScan;
            UseTransactions = useTransactions;
            this.factory = new Lazy<IMongoDatabase>(factory);
        }
    }
}
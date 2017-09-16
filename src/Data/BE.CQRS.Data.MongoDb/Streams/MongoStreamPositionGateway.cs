using System;
using System.Threading.Tasks;
using BE.CQRS.Domain.Denormalization;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Streams
{
    public sealed class MongoStreamPositionGateway : IStreamPositionGateway
    {
        private readonly IMongoCollection<StreamPosition> collection;
        private readonly ILogger<MongoStreamPositionGateway> logger;

        public MongoStreamPositionGateway(IMongoDatabase db, ILoggerFactory loggerFactory)
        {
            collection = db.GetCollection<StreamPosition>("StreamPositions");
            PrepareCollectionAsync().Wait();

            if (loggerFactory != null)
            {
                logger = loggerFactory.CreateLogger<MongoStreamPositionGateway>();
            }
        }

        private Task PrepareCollectionAsync()
        {
            IndexKeysDefinition<StreamPosition> index = Builders<StreamPosition>.IndexKeys.Ascending(x => x.StreamName);

            return collection.Indexes.CreateOneAsync(index);
        }

        public async Task SaveAsync(string streamName, long pos)
        {
            try
            {
                await SaveAsyncInternall(streamName, pos);
            }
            catch (Exception err)
            {
                logger?.LogError("StreamPosition update failed", err);
            }
        }

        private async Task SaveAsyncInternall(string streamName, long pos)
        {
            StreamPosition entry = await GetOrCreate(streamName);

            entry.TimestampUTC = DateTime.UtcNow;
            entry.Position = pos;

            await WriteAsync(streamName, entry);
        }

        private async Task<StreamPosition> GetOrCreate(string streamName)
        {
            StreamPosition entry = await ReadAsync(streamName) ?? new StreamPosition
            {
                Id = streamName.ToLowerInvariant(),
                StreamName = streamName,
                CreatedUTC = DateTime.UtcNow
            };
            return entry;
        }

        public async Task<long?> GetAsync(string streamName)
        {
            StreamPosition entry = await ReadAsync(streamName);

            return entry?.Position;
        }

        private Task<StreamPosition> ReadAsync(string streamName)
        {
            FilterDefinition<StreamPosition> filter = GetStreamFilter(streamName);
            return collection.Find(filter).SingleOrDefaultAsync();
        }

        private Task WriteAsync(string streamName, StreamPosition position)
        {
            FilterDefinition<StreamPosition> filter = GetStreamFilter(streamName);
            return collection.ReplaceOneAsync(filter, position, new UpdateOptions
            {
                IsUpsert = true
            });
        }

        private static FilterDefinition<StreamPosition> GetStreamFilter(string name)
        {
            return Builders<StreamPosition>.Filter.Eq(x => x.StreamName, name);
        }
    }
}
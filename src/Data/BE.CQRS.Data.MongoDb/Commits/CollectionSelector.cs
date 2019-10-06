﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.FluentGuard;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Commits
{
    public sealed class CollectionSelector
    {
        private static readonly IndexKeysDefinitionBuilder<EventCommit> Indexes = Builders<EventCommit>.IndexKeys;

        private readonly ConcurrentDictionary<string, IMongoCollection<EventCommit>> cache =
            new ConcurrentDictionary<string, IMongoCollection<EventCommit>>();

        private readonly IMongoDatabase db;

        public CollectionSelector(IMongoDatabase db)
        {
            this.db = db;
        }

        public IMongoCollection<EventCommit> GetCollectionByAggregateType(string streamName)
        {
            Precondition.For(streamName, nameof(streamName)).NotNullOrWhiteSpace();

            string name = GetCollectionNameByCommit(streamName);

            IMongoCollection<EventCommit> collection = cache.GetOrAdd(name, x => ResolveNew(name).Result);

            return collection;
        }

        private async Task<IMongoCollection<EventCommit>> ResolveNew(string name)
        {
            IMongoCollection<EventCommit> collection = db.GetCollection<EventCommit>(name);
            await PrepareCollection(collection);
            return collection;
        }

        private Task PrepareCollection(IMongoCollection<EventCommit> collection)
        {
            var indexModels = IndexDefinitions.ProvideIndexModels().ToList();

            List<Task> tasks = new List<Task>(indexModels.Count);
            foreach (var model in indexModels)
            {
                tasks.Add(collection.Indexes.CreateOneAsync(model));
            }

            return Task.WhenAll(tasks);
        }

        public static string GetCollectionNameByCommit(string type)
        {
            return $"ag_{type}";
        }
    }
}
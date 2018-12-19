using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.FluentGuard;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Repositories
{
    public abstract class MongoRepositoryBase<TDto> where TDto : class, new()
    {
        private static readonly Type VersionInterfaceType;

        protected static FilterDefinitionBuilder<TDto> Filters { get; } = Builders<TDto>.Filter;

        protected static UpdateDefinitionBuilder<TDto> Updates { get; } = Builders<TDto>.Update;

        protected static IndexKeysDefinitionBuilder<TDto> Indexes { get; } = Builders<TDto>.IndexKeys;

        protected static SortDefinitionBuilder<TDto> Sorts { get; } = Builders<TDto>.Sort;

        protected IMongoCollection<TDto> Collection { get; }

        protected IMongoDatabase Database { get; }

        private static readonly Type dtoType;
        private static readonly bool isVersioned;

        private static readonly bool isReadObject;

        static MongoRepositoryBase()

        {
            VersionInterfaceType = typeof(IVersioned);
            dtoType = typeof(TDto);

            isReadObject = typeof(IReadDto).IsAssignableFrom(dtoType);
            isVersioned = VersionInterfaceType.IsAssignableFrom(dtoType);
        }

        protected MongoRepositoryBase(IMongoDatabase database, string collectionName)
        {
            Precondition.For(database, nameof(database)).NotNull();
            Precondition.For(collectionName, nameof(collectionName)).NotNullOrWhiteSpace().MinLength(3);

            Database = database;
            Collection = database.GetCollection<TDto>(collectionName);
        }

        public Task InsertNewDtoAsync(TDto dto)
        {
            Precondition.For(dto, nameof(dto)).NotNull();

            SetInitialVersion(dto);

            if (isReadObject)
            {
                var temp = (IReadDto)dto;
                DateTime date = DateTime.UtcNow;
                temp.WriteCreateTimestampUtc = date;
                temp.WriteTimestampUtc = date;
            }

            return Collection.InsertOneAsync(dto);
        }

        private static void SetInitialVersion(TDto dto)
        {
            if (isVersioned)
            {
                var temp = dto as IVersioned;
                temp.Version = 0;
            }
        }

        public Task UpdateDtoAsync(FilterDefinition<TDto> query, UpdateDefinition<TDto> update)
        {
            if (isVersioned)
            {
                update = update.Inc("Version", 1);
            }

            if (isReadObject)
            {
                update = update.Set("WriteTimestampUtc", DateTime.UtcNow);
            }

            return Collection.UpdateOneAsync(query, update);
        }

        protected Task CreateIndexesAsync(params CreateIndex<TDto>[] definitions)
        {
            if (definitions.Length <= 0)
            {
                return Task.FromResult(true);
            }

            List<Task<string>> tasks = definitions.Select(CreateIndexAsync).ToList();

            return Task.WhenAll(tasks);
        }

        protected Task<string> CreateIndexAsync(CreateIndex<TDto> index)
        {
            Precondition.For(index, nameof(index)).NotNull();

            Task<string> task = index.Options == null
                ? Collection.Indexes.CreateOneAsync(new CreateIndexModel<TDto>(null,index.Options))
                : Collection.Indexes.CreateOneAsync(new CreateIndexModel<TDto>(index.Definition, index.Options));
            return task;
        }
    }
}
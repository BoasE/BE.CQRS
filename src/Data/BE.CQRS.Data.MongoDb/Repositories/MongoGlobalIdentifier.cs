using System.Threading.Tasks;
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Repositories
{
    public sealed class MongoGlobalIdentifier : MongoRepositoryBase<IdDto>
    {
        public MongoGlobalIdentifier(IMongoDatabase database) : base(database, "es_id")
        {
        }

        public async Task<long> Next(string scope)
        {
            UpdateDefinition<IdDto> def = Updates.Inc(x => x.Value, 1);

            FilterDefinition<IdDto> filter = Filters.Eq(x => x.Scope, scope);

            IdDto response = await Collection
                .FindOneAndUpdateAsync(filter, def, new FindOneAndUpdateOptions<IdDto, IdDto>
                {
                    ReturnDocument = ReturnDocument.After,
                    IsUpsert = true
                });

            return response.Value;
        }
    }
}
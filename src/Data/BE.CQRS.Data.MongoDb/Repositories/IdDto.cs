using MongoDB.Bson.Serialization.Attributes;

namespace BE.CQRS.Data.MongoDb.Repositories
{
    [BsonIgnoreExtraElements]
    public class IdDto
    {
        [BsonRequired]
        public long Value { get; set; }

        [BsonRequired]
        public string Scope { get; set; }
    }
}
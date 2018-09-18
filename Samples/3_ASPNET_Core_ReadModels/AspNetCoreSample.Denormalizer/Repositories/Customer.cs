using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AspNetCoreSample.Denormalizer.Repositories
{
    public sealed class Customer
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}
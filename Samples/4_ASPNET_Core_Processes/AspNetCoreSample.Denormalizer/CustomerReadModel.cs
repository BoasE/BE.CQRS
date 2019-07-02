using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AspNetCoreSample.Denormalizer
{
    public sealed class CustomerReadModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string CustomerId { get; set; }
        public string Name { get; set; }
    }
}
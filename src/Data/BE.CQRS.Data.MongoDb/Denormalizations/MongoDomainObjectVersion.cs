using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BE.CQRS.Data.MongoDb.Denormalizations
{
    [BsonIgnoreExtraElements]
    public sealed class MongoDomainObjectVersion
    {
        [BsonId] public ObjectId Id { get; set; }

        [Required] public string DomainObjectType { get; set; }

        [Required] public long Version { get; set; }
    }
}
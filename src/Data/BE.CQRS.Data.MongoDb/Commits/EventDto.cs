using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace BE.CQRS.Data.MongoDb.Commits
{
    public sealed class EventDto
    {
        [BsonElement("id")]
        [BsonRequired]
        public string Id { get; set; }

        [BsonElement("h")]
        [BsonRequired]
        public Dictionary<string, string> Headers { get; set; }

        [BsonRequired]
        [BsonElement("b")]
        public string Body { get; set; }
    }
}
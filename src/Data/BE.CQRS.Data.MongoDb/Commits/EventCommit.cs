using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BE.CQRS.Data.MongoDb.Commits
{
    [BsonIgnoreExtraElements]
    public sealed class EventCommit
    {
        [BsonRequired]
        [BsonElement("_sts", Order = 1)]
        public BsonTimestamp ServerTimestamp { get; set; } = new BsonTimestamp(0);

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("aty", Order = 3)]
        public string AggregateType { get; set; }

        [BsonRequired]
        [BsonElement("aid", Order = 4)]
        public string AggregateId { get; set; }

        [BsonRequired]
        [BsonElement("atp")]
        public string AggregatePackage { get; set; }

        [BsonRequired]
        [BsonElement("atys")]
        public string AggregateTypeShort { get; set; }

        [BsonRequired]
        [BsonElement("ord")]
        public long Ordinal { get; set; }

        [BsonRequired]
        [BsonElement("tst")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; }

        [BsonRequired]
        [BsonElement("ve")]
        public long VersionEvents { get; set; }

        [BsonRequired]
        [BsonElement("vc")]
        public long VersionCommit { get; set; }

        [BsonRequired]
        [BsonElement("eve")]
        public Dictionary<string, EventDto> Events { get; set; }
    }
}
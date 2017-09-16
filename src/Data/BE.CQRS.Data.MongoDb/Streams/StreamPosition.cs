using System;
using MongoDB.Bson.Serialization.Attributes;

namespace BE.CQRS.Data.MongoDb.Streams
{
    public sealed class StreamPosition
    {
        [BsonId]
        public string Id { get; set; }

        [BsonRequired]
        public string StreamName { get; set; }

        [BsonRequired]
        public long Position { get; set; }

        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        // ReSharper disable once InconsistentNaming
        public DateTime TimestampUTC { get; set; }

        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        // ReSharper disable once InconsistentNaming
        public DateTime CreatedUTC { get; set; }
    }
}
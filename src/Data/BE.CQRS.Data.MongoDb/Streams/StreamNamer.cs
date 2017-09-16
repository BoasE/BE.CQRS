using System;
using BE.CQRS.Domain.DomainObjects;

namespace BE.CQRS.Data.MongoDb.Streams
{
    public sealed class StreamNamer
    {
        private readonly string seperator = "_";

        public string GetCollectionName(IDomainObject source)
        {
            return string.Concat(source.Id, "_events");
        }

        public string ResolveStreamName(string id, Type aggregateType)
        {
            return string.Concat(id, seperator, aggregateType.FullName);
        }

        public string TypeNameByStreamName(string streamName)
        {
            int index = streamName.IndexOf(seperator, StringComparison.OrdinalIgnoreCase);
            return streamName.Substring(index + 1);
        }

        public string IdByStreamName(string streamName)
        {
            int index = streamName.IndexOf(seperator, StringComparison.OrdinalIgnoreCase);
            return streamName.Substring(0, index);
        }
    }
}
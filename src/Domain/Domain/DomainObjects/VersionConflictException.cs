using System;

namespace BE.CQRS.Domain.DomainObjects
{
    public sealed class VersionConflictException : Exception
    {
        public string AggregateType { get; set; }

        public string Id { get; set; }

        public long Version { get; set; }

        public override string Message => $"Version conflict for {AggregateType} - {Id} with version {Version}";

        public VersionConflictException()
        {
        }

        public VersionConflictException(string aggregateType, string id, long version)
        {
            AggregateType = aggregateType;
            Id = id;
            Version = version;
        }
    }
}
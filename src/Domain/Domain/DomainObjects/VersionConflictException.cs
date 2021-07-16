using System;

namespace BE.CQRS.Domain.DomainObjects
{
    public sealed class VersionConflictException : Exception
    {
        public string AggregateType { get; set; }

        public string Id { get; set; }

        public long DbVersion { get; set; }
        
        public override string Message => $"Version conflict for {AggregateType} - {Id} with DbVersion {DbVersion}";

        public VersionConflictException()
        {
        }

        public VersionConflictException(string aggregateType, string id, long dbVersion)
        {
            AggregateType = aggregateType;
            Id = id;
            DbVersion = dbVersion;
        }
    }
}
using System;

namespace BE.CQRS.Data.MongoDb.Repositories
{
    public interface IReadDto
    {
        DateTime WriteTimestampUtc { get; set; }

        DateTime WriteCreateTimestampUtc { get; set; }
    }
}
using MongoDB.Driver;

namespace BE.CQRS.Data.MongoDb.Repositories
{
    public static class VersionExtensions
    {
        public static UpdateDefinition<T> IncrementVersion<T>(this UpdateDefinitionBuilder<T> builder)
            where T : IVersioned
        {
            return builder.Inc(x => x.Version, 1);
        }
    }
}
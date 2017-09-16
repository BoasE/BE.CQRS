namespace BE.CQRS.Data.MongoDb.Repositories
{
    public interface IVersioned
    {
        int Version { get; set; }
    }
}
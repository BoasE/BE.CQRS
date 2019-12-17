namespace BE.CQRS.Domain.Events
{
    public interface IEventHash
    {
        byte[] Hash(string body);
        
        string HashString(string body);
    }
}
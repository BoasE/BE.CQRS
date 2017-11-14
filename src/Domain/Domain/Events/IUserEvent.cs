namespace BE.CQRS.Domain.Events
{
    public interface IUserEvent : IEvent
    {
        string UserId { get; set; }
    }
}
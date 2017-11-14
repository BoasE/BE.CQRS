namespace BE.CQRS.Domain.Commands
{
    public interface IUserCommand : ICommand
    {
        string UserId { get; }
    }
}
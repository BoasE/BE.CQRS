namespace BE.CQRS.Domain.Commands
{
    public interface ICommand
    {
        string DomainObjectId { get; }
    }
}
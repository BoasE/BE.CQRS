using BE.CQRS.Domain.Commands;

namespace Tests.Fakes
{
    public class CreateCommand : ICommand
    {
        public string DomainObjectId { get; set; }
    }
}
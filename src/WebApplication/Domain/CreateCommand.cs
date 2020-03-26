using BE.CQRS.Domain.Commands;

namespace WebApplication.Domain
{
    public class CreateCommand : CommandBase
    {
        public string Value { get; set; }

        public CreateCommand(string domainObjectId) : base(domainObjectId)
        {
        }
    }
}
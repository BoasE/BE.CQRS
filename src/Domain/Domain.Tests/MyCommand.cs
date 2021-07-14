using BE.CQRS.Domain.Commands;

namespace BE.CQRS.Domain.Tests
{
    public class MyCommand : CommandBase
    {
        public MyCommand(string domainObjectId) : base(domainObjectId)
        {
        }
    }
}
using BE.CQRS.Domain.Commands;

namespace Domain.Commands
{
    public sealed class CreateCustomerFromConsoleCommand : ICommand // All Commands must be dervied from ICommand
    {
        public string DomainObjectId { get; set; }

        public string Name { get; set; }
        
    }
}

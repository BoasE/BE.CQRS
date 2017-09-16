using BE.CQRS.Domain.Commands;

namespace RepositorySamples.EventStore.Domain
{
    public sealed class CreatCustomerFromConsoleCommand : ICommand // All Commands must be dervied from ICommand
    {
        public string DomainObjectId { get; set; }

        public string Name { get; set; }
        
    }
}

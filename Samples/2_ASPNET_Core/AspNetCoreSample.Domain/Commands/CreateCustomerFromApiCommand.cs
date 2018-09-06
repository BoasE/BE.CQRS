using BE.CQRS.Domain.Commands;

namespace AspNetCoreSample.Domain.Commands
{
    public sealed class CreateCustomerFromApiCommand : ICommand // All Commands must be dervied from ICommand
    {
        public string DomainObjectId { get; set; }

        public string Name { get; set; }
        
    }
}

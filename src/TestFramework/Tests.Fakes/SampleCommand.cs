using BE.CQRS.Domain.Commands;

namespace Tests.Fakes
{
    public sealed class SampleCommand : ICommand
    {
        public string DomainObjectId { get; } = "sfdsfds";

        public int Value { get; set; }
    }
}
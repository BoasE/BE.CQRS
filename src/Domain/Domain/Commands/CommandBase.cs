using BE.FluentGuard;

namespace BE.CQRS.Domain.Commands
{
    public abstract class CommandBase : ICommand
    {
        public string DomainObjectId { get; }

        protected CommandBase(string domainObjectId)
        {
            Precondition.For(domainObjectId, nameof(domainObjectId)).NotNullOrWhiteSpace();

            DomainObjectId = domainObjectId;
        }
    }
}
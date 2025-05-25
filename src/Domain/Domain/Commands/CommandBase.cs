using System;
using BE.FluentGuard;

namespace BE.CQRS.Domain.Commands
{
    public abstract class CommandBase : ICommand
    {
        public string CommandId { get; } = Guid.CreateVersion7().ToString();
        public string DomainObjectId { get; }

        protected CommandBase(string domainObjectId)
        {
            Precondition.For(domainObjectId, nameof(domainObjectId)).NotNullOrWhiteSpace();

            DomainObjectId = domainObjectId;
        }
    }
}
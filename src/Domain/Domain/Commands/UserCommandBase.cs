using BE.FluentGuard;

namespace BE.CQRS.Domain.Commands
{
    public abstract class UserCommandBase : CommandBase, IUserCommand
    {
        public string UserId { get; }

        protected UserCommandBase(string domainObjectId, string userId) : base(domainObjectId)
        {
            Precondition.For(domainObjectId, nameof(domainObjectId)).NotNullOrWhiteSpace();
            Precondition.For(userId, nameof(userId)).NotNullOrWhiteSpace();

            UserId = userId;
        }
    }
}
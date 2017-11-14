using BE.FluentGuard;

namespace BE.CQRS.Domain.Events
{
    public abstract class UserEventBase : EventBase, IUserEvent
    {
        public string UserId { get; set; }

        protected UserEventBase()
        {
        }

        protected UserEventBase(string userId)
        {
            SetUserId(userId);
        }

        protected UserEventBase(string userId, EventHeader header) : base(header)
        {
            SetUserId(userId);
        }

        private void SetUserId(string userId)
        {
            Precondition.For(userId, nameof(userId)).NotNullOrWhiteSpace();
            UserId = userId;
            Headers.Set(EventHeaderKeys.UserId, userId);
        }
    }
}
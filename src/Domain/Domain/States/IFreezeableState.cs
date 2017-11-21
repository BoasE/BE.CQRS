namespace BE.CQRS.Domain.States
{
    public interface IFreezeableState : IState
    {
        bool IsFrozen { get; }

        void Freeze();
    }
}
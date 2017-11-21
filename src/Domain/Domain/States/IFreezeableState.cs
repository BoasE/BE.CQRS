namespace BE.CQRS.Domain.States
{
    public interface IFreezeableState : IState
    {
        bool IsFreezed { get; }

        void Freeze();
    }
}
using BE.CQRS.Domain.Commands;
using BE.CQRS.Domain.States;

namespace BE.CQRS.Domain.Policies
{
    public abstract class PolicyBase : StateBase, IPolicy
    {
        protected PolicyBase()
        {
        }

        public abstract bool IsValid();
    }

    public abstract class CommandPolicyBase<TCommand> : PolicyBase where TCommand : ICommand
    {
        public TCommand Command { get; private set; }

        protected CommandPolicyBase(TCommand command)
        {
            Command = command;
        }
    }
}
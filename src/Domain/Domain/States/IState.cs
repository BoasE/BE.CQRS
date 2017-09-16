using System.Collections.Generic;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.States
{
    public interface IState
    {
        void Execute(IEnumerable<IEvent> source);
    }
}
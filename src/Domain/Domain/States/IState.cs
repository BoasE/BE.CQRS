using System;
using System.Collections.Generic;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.States
{
    public interface IState
    {
        TimeSpan Duration { get; }
        int AppliedEvents { get; }
        int InvokedMethods { get; }

        void Execute(IEnumerable<IEvent> source);
    }
}
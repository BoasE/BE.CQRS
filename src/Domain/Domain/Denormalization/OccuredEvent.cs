using System;
using BE.CQRS.Domain.Events;

namespace BE.CQRS.Domain.Denormalization
{
    public sealed class OccuredEvent : EventArgs
    {
        public long Pos { get; }

        public IEvent Event { get; }

        public OccuredEvent(long pos, IEvent @event)
        {
            Pos = pos;
            Event = @event;
        }
    }
}
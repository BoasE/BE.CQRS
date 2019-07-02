using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.CQRS.Domain.Events;
using BE.CQRS.Domain.Events.Handlers;
using BE.FluentGuard;

namespace BE.CQRS.Domain.States
{
    public abstract class StateBase : IFreezeableState
    {
        private static readonly IEventMethodConvetion MethodConvetion = new OnPrefixEventMethodConvetion();
        private static readonly EventHandlerInvoker Invoker = new EventHandlerInvoker();

        private bool breakRequested;

        private readonly IEventHandlerRegistry registry;

        public bool IsFrozen { get; private set; }

        protected StateBase()
        {
            IsFrozen = false;
            EventHandlerMethod[] methods = MethodConvetion.ResolveEventMethods(GetType()).ToArray();
            registry = new EventHandlerRegistry();
            registry.Add(methods);
        }

        public void Execute(IEnumerable<IEvent> source)
        {
            Precondition.For(source, nameof(source)).NotNull();

            if (IsFrozen)
            {
                throw new InvalidOperationException("State is frozen and can't be executed again!");
            }

            foreach (IEvent entry in source)
            {
                On(entry);

                if (breakRequested)
                {
                    break;
                }
            }

            Freeze();
        }

        protected void Break()
        {
            breakRequested = true;
        }

        protected virtual void On(IEvent @event)
        {
            InvokeMethods(@event).Wait();
        }

        private async Task InvokeMethods(IEvent @event)
        {
            EventHandlerMethod[] methods = registry.Resolve(@event.GetType());

            foreach (EventHandlerMethod method in methods)
            {
                await Invoker.InvokeAsync(@event, method, this); //Todo get events
            }
        }

        public void Freeze()
        {
            IsFrozen = true;
        }
    }
}